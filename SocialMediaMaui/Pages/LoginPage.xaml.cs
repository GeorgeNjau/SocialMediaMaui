namespace SocialMediaMaui.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void btnRegister_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(RegisterPage), animate: true);
	}

	private async void btnLogin_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"//{nameof(HomePage)}", animate: true);
	}
}