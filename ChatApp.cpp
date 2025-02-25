// ---------------------------------------------------------------------
// Copyright (c) 2024 Qualcomm Innovation Center, Inc. All rights reserved.
// SPDX-License-Identifier: BSD-3-Clause
// ---------------------------------------------------------------------
#include "ChatApp.hpp"
#include "PromptHandler.hpp"
#include <fstream>
#include <iostream>
#include <windows.h>

using namespace App;

namespace
{

constexpr const int c_chat_separater_length = 80;
HANDLE hPipe;
char buffer[1024*1024*64];

int StartNamedPipeServer(void)
{
   

    DWORD dwRead;

    hPipe = CreateNamedPipe(TEXT("\\\\.\\pipe\\CivilDialogChatAppPipe"), PIPE_ACCESS_DUPLEX,
                            PIPE_TYPE_BYTE | PIPE_READMODE_BYTE |
                                PIPE_WAIT, // FILE_FLAG_FIRST_PIPE_INSTANCE is not needed but forces CreateNamedPipe(..)
                                           // to fail if the pipe already exists...
                            1, 1024 * 1024, 1024 * 1024, NMPWAIT_USE_DEFAULT_WAIT, NULL);
    return 0;
}


int EndNamedPipeServer(void)
{
    DisconnectNamedPipe(hPipe);
    return 0;
}


//
// ChatSplit - Line to split during Chat for UX
// Adds split line to separate out sections in output.
//
void ChatSplit(bool end_line = true)
{
    std::string split_line(c_chat_separater_length, '-');
    std::cout << "\n" << split_line;
    if (end_line)
    {
        std::cout << "\n";
    }
}

//
// GenieCallBack - Callback to handle response from Genie
//   - Captures response from Genie into user_data
//   - Print response to stdout
//   - Add ChatSplit upon sentence completion
//
void GenieCallBack(const char* response_back, const GenieDialog_SentenceCode_t sentence_code, const void* user_data)
{
    std::string* user_data_str = static_cast<std::string*>(const_cast<void*>(user_data));
    user_data_str->append(response_back);

    // Write user response to output.
    //std::cout << response_back;
    if (sentence_code == GenieDialog_SentenceCode_t::GENIE_DIALOG_SENTENCE_END)
    {
        ChatSplit(false);
    }
}

} // namespace

ChatApp::ChatApp(const std::string& config)
{
    // Create Genie config
    if (GENIE_STATUS_SUCCESS != GenieDialogConfig_createFromJson(config.c_str(), &m_config_handle))
    {
        throw std::runtime_error("Failed to create the Genie Dialog config. Please check config.");
    }

    // Create Genie dialog handle
    if (GENIE_STATUS_SUCCESS != GenieDialog_create(m_config_handle, &m_dialog_handle))
    {
        throw std::runtime_error("Failed to create the Genie Dialog.");
    }
}

ChatApp::~ChatApp()
{
    if (m_config_handle != nullptr)
    {
        if (GENIE_STATUS_SUCCESS != GenieDialogConfig_free(m_config_handle))
        {
            std::cerr << "Failed to free the Genie Dialog config.";
        }
    }

    if (m_dialog_handle != nullptr)
    {
        if (GENIE_STATUS_SUCCESS != GenieDialog_free(m_dialog_handle))
        {
            std::cerr << "Failed to free the Genie Dialog.";
        }
    }
}

void ChatApp::ChatWithUser(/*const std::string& user_name*/)
{
    AppUtils::PromptHandler prompt_handler;
    StartNamedPipeServer();
    // Initiate Chat with infinite loop.
    // User to provide `exit` as a prompt to exit.
    while (true)
    {
        std::string user_prompt;
        std::string model_response;

        // Input user prompt
        ChatSplit();
        //std::cout << user_name << ": ";
        //std::getline(std::cin, user_prompt);
        if (hPipe != INVALID_HANDLE_VALUE)
        {
            DWORD dwRead;
            
            if (ConnectNamedPipe(hPipe, NULL) != FALSE) // wait for someone to connect to the pipe
            {
                while (ReadFile(hPipe, buffer, sizeof(buffer) - 1, &dwRead, NULL) != FALSE)
                {
                    /* add terminating zero */
                    buffer[dwRead] = '\0';

                    /* do something with data in buffer */
                    //printf("%s", buffer);
                }
                DisconnectNamedPipe(hPipe);

                //user_prompt = new std::string(buffer,);
                user_prompt = (static_cast<const char*>(buffer));
                //std::string user_prompt(static_cast<const char*>(buffer), dwRead);
            }
        }

        // Exit prompt
        if (user_prompt.compare(c_exit_prompt) == 0)
        {
            std::cout << "Exiting chat...";
            EndNamedPipeServer();
            return;
        }
        // User provides an empty prompt
        if (user_prompt.empty())
        {
            //std::cout << "\nPlease enter prompt.\n";
            continue;
        }
        else
        {
            std::cout << "\nAnalyzing prompt:\n";
            std::cout << user_prompt;
            
        }

        std::string tagged_prompt = prompt_handler.GetPromptWithTag(user_prompt);

        // Bot's response
        std::cout << c_bot_name << ":";
        if (GENIE_STATUS_SUCCESS != GenieDialog_query(m_dialog_handle, tagged_prompt.c_str(),
                                                      GenieDialog_SentenceCode_t::GENIE_DIALOG_SENTENCE_COMPLETE,
                                                      GenieCallBack, &model_response))
        {
            throw std::runtime_error("Failed to get response from GenieDialog. Please restart the ChatApp.");
        }

        if (model_response.empty())
        {
            std::cout << "**********************************  CHAT RESET ********************************************************* ";
            // If model response is empty, reset dialog to re-initiate dialog.
            // During local testing, we found that in certain cases,
            // model response bails out after few iterations during chat.
            // If that happens, just reset Dialog handle to continue the chat.
            if (GENIE_STATUS_SUCCESS != GenieDialog_reset(m_dialog_handle))
            {
                throw std::runtime_error("Failed to reset Genie Dialog.");
            }
            if (GENIE_STATUS_SUCCESS != GenieDialog_query(m_dialog_handle, tagged_prompt.c_str(),
                                                          GenieDialog_SentenceCode_t::GENIE_DIALOG_SENTENCE_COMPLETE,
                                                          GenieCallBack, &model_response))
            {
                throw std::runtime_error("Failed to get response from GenieDialog. Please restart the ChatApp.");
            }
        }
        DisconnectNamedPipe(hPipe);

        if (ConnectNamedPipe(hPipe, NULL) != FALSE)
        {
            DWORD dwWrite;
            if (!model_response.empty())
            {
                std::cout << model_response;
                WriteFile(hPipe, model_response.c_str(), model_response.size(), &dwWrite, NULL);
                FlushFileBuffers(hPipe);
            }
            //std::string response = "<!End!>";
            //WriteFile(hPipe, response.c_str(), response.size(), &dwWrite, NULL); 
            DisconnectNamedPipe(hPipe);
        }
    }
}
