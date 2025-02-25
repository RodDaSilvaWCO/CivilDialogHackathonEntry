using CivilDialog;
using CivilDialog.Services;
using CivilDialog.ViewModels;

namespace CivilDialog
{
    //public partial class App : Application
    //{
    //    public App()
    //    {
    //        InitializeComponent();

    //        MainPage = new MainPage();
    //    }
    //}
    public partial class App : Application, IDisposable
    {
        private ICivilDialogHubService _civilDialogHubService;
        internal NavigationService _navigatorService = null!;
        public App(NavigationService navigationService, PostCommentViewModel postCommentViewModel, ICivilDialogHubService civilDialogHubService)
        {
            _navigatorService = navigationService;

            InitializeComponent();
            BindingContext = postCommentViewModel;
            //_civilDialogHubService = new CivilDialogHubService();
            _civilDialogHubService = civilDialogHubService;
            _civilDialogHubService.RegisterMessageHandler();
            _civilDialogHubService.StartConnectionAsync();
            //_civilDialogHubService.PostReplyStart += OnPostReplyStarted;
            //async (sender, e) =>
            //{
            //    //await Shell.Current.GoToAsync(nameof(DisplayAlert)).ConfigureAwait(false);
            //};  
            //    _httpClient = new HttpClient();

            Routing.RegisterRoute(nameof(DisplayAlert), typeof(DisplayAlert));
            //MainPage = new MainPage();
        }

        //void OnPostReplyStarted(object sender, HubEventArgs e)
        //{

        //}



        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();
        //    await _signalRService.StartConnectionAsync();
        //}

        //protected override 



        private void MenuItem_Clicked(object sender, EventArgs e)
        {

            var menuItem = (MenuItem)sender;
            var url = menuItem.Text switch
            {
                "Counter" => "/counter",
                "Weather" => "/weather",
                _ => "/"
            };

            _navigatorService?.NavigationManager?.NavigateTo(url);

        }

        public void Dispose()
        {
            _civilDialogHubService.StopConnectionAsync().Wait();
        }
        //private async void PostButton_Clicked(object sender, EventArgs e)
        //{
        //    //await Shell.Current.GoToAsync(nameof(DisplayAlert)).ConfigureAwait(false);
        //    HttpClient _httpClient = new HttpClient();
        //    //var text = ((Editor)Shell.Current.FindByName("ReplyEditor")).Text;
        //    //var text = text;
        //    //var content = new StringContent(text, Encoding.UTF8, "application/json");    
        //    //78676tvar response = await _httpClient.PostAsync("https://localhost:7003/api/ForumPost", content);
        //}
    }
}
