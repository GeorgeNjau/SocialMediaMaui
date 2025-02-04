namespace SocialMediaMaui.Pages;

public partial class PostDetailPage : ContentPage
{
	public PostDetailPage()
	{
		InitializeComponent();
	}

	private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
	{
		await Shell.Current.GoToAsync("..", animate: true);
	}
}