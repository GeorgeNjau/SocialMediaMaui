﻿namespace SocialMediaMaui.Pages;

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

    private async void NewPostImage_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddPostPage), animate: true);
    }

    private async void GoToProfilePage_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ProfilePage), animate: true);
    }

    private async void GoToNotificationPage_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NotificationPage), animate: true);
    }
}