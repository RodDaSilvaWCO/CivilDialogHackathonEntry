using CivilDialog.Services;

namespace CivilDialog;

public partial class DisplayAlert : ContentPage
{
    //private NavigationService _navigationService = null!;

    public DisplayAlert(/*NavigationService navigationService*/)
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.Current.Navigating += OnNavigating;
        
    }

    protected override void OnDisappearing()
    {
        Shell.Current.Navigating -= OnNavigating;
        base.OnDisappearing();
    }

    

    private async void OnNavigating(object sender, ShellNavigatingEventArgs args)
    {
        await Task.CompletedTask;
        args.Cancel();
    }

        //protected override void OnAppearing()
        //{
        //    ((App)Application.Current!)._navigatorService?.NavigationManager?.NavigateTo("/counter");
        //    Naviga09ion.PopToRootAsync().Wait();
        //    //Navigation.PopAsync();
        //    //base.OnAppearing();
        //    //await DisplayAlert("Alert", "You have been alerted", "OK");
        //}
}