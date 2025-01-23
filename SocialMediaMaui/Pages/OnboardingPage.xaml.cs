namespace SocialMediaMaui.Pages;

public partial class OnboardingPage : ContentPage
{
	public OnboardingPage()
	{
		InitializeComponent();
	}

	private async void btnGetStarted_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
	}
}