namespace SocialMediaMaui.Pages;

public partial class NotificationPage : ContentPage
{
	public NotificationPage()
	{
		InitializeComponent();

		List<NotificationModel> notifications = [
			new NotificationModel(DateTime.Now, "This person licked your post"),
			new NotificationModel(DateTime.Now.AddMinutes(-1), "This person licked your post"),
			new NotificationModel(DateTime.Now.AddMinutes(-30), "This person bookmarked your post"),
			new NotificationModel(DateTime.Now.AddHours(-2), "This person licked your post"),
			new NotificationModel(DateTime.Now.AddHours(-7), "This person licked your post"),
			new NotificationModel(DateTime.Now.AddDays(-1), "This person licked your post"),
			new NotificationModel(DateTime.Now.AddDays(-2), "This person bookmarked your post"),
			new NotificationModel(DateTime.Now.AddDays(-2), "This person licked your post"),
			new NotificationModel(DateTime.Now.AddDays(-10), "This person licked your post"),
			];

		notificationsCollection.ItemsSource = notifications;

	}

	public record NotificationModel(DateTime On, string Text);
}