namespace Camera.MAUI.Test;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
    private async void Button_Clicked_Params(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SizedPage());
    }

    private async void Button_Clicked_Fullscreen(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FullScreenPage());
    }

    private async void Button_Clicked_CodeGen(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new BarcodeGenerationPage());
    }

    private async void Button_Clicked_MVVM(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MVVMPage());
    }
}