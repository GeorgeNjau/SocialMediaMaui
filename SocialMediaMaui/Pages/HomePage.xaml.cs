namespace SocialMediaMaui.Pages;

public partial class HomePage : ContentPage
{
    int count = 0;

    public HomePage()
    {
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PostDetailPage), animate: true);
    }
}