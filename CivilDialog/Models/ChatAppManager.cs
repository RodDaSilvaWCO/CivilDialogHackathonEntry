using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CivilDialog.Interfaces;

namespace CivilDialog.Models
{
    public class ChatAppManager : IChatAppManager, IDisposable
    {
        #region Member Fields
        private string _executableDirectory;
        private string _executableName;
        private string _commandLineArgs;
        private int _procID = -1;
        #endregion Member Fields

        #region Constructors
        public ChatAppManager(string executableDirectory, string executableName, string commandLineArgs)
        { 
            _executableDirectory = executableDirectory;
            _executableName = executableName;
            _commandLineArgs = commandLineArgs;

            
        }
        #endregion Constructors


        #region IDisposble Implementation
        public void Dispose()
        {

        }
        #endregion 

        #region IChatAppManager Implementation
        public bool IsInitialized 
        {
            get 
            { 
                return IsProcessRunning();  
            } 
        }

        public async Task<bool> CreateInstanceAsync()
        {
            bool result = false;
            try
            {
                _procID = LaunchChatAppAsync(); 
                result = (_procID > 0);
            }
            catch (Exception ex)
            {
                result = false;
                Debug.Print($"*** Error attempting to start executableName, {ex}  ");
            }
            return await Task.FromResult( result );
        }

        public void ShutDownInstance()
        {
            while(IsInitialized)
            {
                NamedPipeClientStream pipeClient = null!;
                try
                {
                    pipeClient =
                        new NamedPipeClientStream(".", "CivilDialogChatAppPipe",
                            PipeDirection.InOut, PipeOptions.None,
                            TokenImpersonationLevel.None);

                    pipeClient.Connect(1000);
                    var ss = new StreamString(pipeClient);
                    ss.WriteString("exit");
                    pipeClient.Close();
                }
                catch (Exception ex)
                {

                }
                finally 
                { 
                    if( pipeClient != null && pipeClient.IsConnected)
                    {
                        pipeClient.Close();
                    }
                }
            }
            _procID = -1;
        }



        public async Task<string> RunInferenceAsync( string prompt )
        {
            string result = "";
            try
            {
                //while (IsInitialized)
                //{
                    NamedPipeClientStream pipeClient = null!;
                    try
                    {
                        pipeClient =
                            new NamedPipeClientStream(".", "CivilDialogChatAppPipe",
                                PipeDirection.InOut, PipeOptions.None,
                                TokenImpersonationLevel.None);

                        pipeClient.Connect(1000);
                        var ss = new StreamString(pipeClient);
                        ss.WriteString(prompt);
                        pipeClient.Close();

                        pipeClient =
                            new NamedPipeClientStream(".", "CivilDialogChatAppPipe",
                        PipeDirection.InOut, PipeOptions.None,
                        TokenImpersonationLevel.None);
                        pipeClient.Connect();//pipeClient.Connect();
                        ss = new StreamString(pipeClient);
                        result = ss.ReadString();
                        pipeClient.Close();
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        if (pipeClient != null && pipeClient.IsConnected)
                        {
                            pipeClient.Close();
                        }
                    }
                //}
            }
            catch (Exception)
            {

                throw;
            }
            return await Task.FromResult(result);
        }
        #endregion IChatAppManager Implementation


        #region Static Methods
        //public static ChatAppManager CreateInstance()
        //{

        //}
        #endregion 

        #region Helpers
        private int LaunchChatAppAsync()
        {
            Process proc = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.WorkingDirectory = _executableDirectory;
            psi.FileName = Path.Combine(_executableDirectory, _executableName + ".exe");
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.Arguments = _commandLineArgs;
            proc.StartInfo = psi;
            proc.Start();
            return proc.Id;
        }

        private bool IsProcessRunning()
        {
            bool result = false;
            var nodeExecutable = _executableName.ToUpper();
            try
            {
                var p = Process.GetProcessById(_procID);
                if (p.ProcessName.ToUpper().IndexOf(nodeExecutable) == 0)
                {
                    // Found a process in the NetworkSpec still running...
                    result = true;
                }
            }
            catch (Exception)
            {
                // NOP and continue
            }
            return result;
        }
        #endregion 

    }

    public class StreamString
    {
        private Stream ioStream;
        private ASCIIEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new ASCIIEncoding();
        }

        public string ReadString()
        {
            string response = "";
            byte[] buffer = new byte[1024 * 1024 * 5];
            int bytesRead = 0;
            while (response.IndexOf("<!End!>") < 0)
            {
                var bytesReadThisChunk = ioStream.Read(buffer, bytesRead, buffer.Length - bytesRead);
                response += streamEncoding.GetString(buffer, bytesRead, bytesReadThisChunk);
                bytesRead += bytesReadThisChunk;
                if (bytesReadThisChunk == 0)
                    break;
            }
            return response;
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length;
        }
    }
}
