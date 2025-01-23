using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Runtime.CompilerServices;

namespace SocialMediaMaui.Controls
{
    public class NoBorderEntry : Entry
    {
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            SetBorderlessBackground();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
#if ANDROID

            if (propertyName == nameof(BackgroundColor))
                SetBorderlessBackground();

#endif
        }

        private void SetBorderlessBackground()
        {
#if ANDROID
            if (Handler is IEntryHandler entryHandler)
            {
                if (BackgroundColor == null)
                    entryHandler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
                else
                    entryHandler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(BackgroundColor.ToPlatform());
            }
#endif
        }
    }

}
