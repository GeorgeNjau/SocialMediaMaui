
namespace SocialMediaMaui.Controls
{
    public class Spacer : ContentView
    {
        public BoxView Box { get; set; }

        public Spacer()
        {
            Box = new BoxView()
            {
                BackgroundColor = Colors.Transparent,
                HeightRequest = 20
            };

            Content = Box;
        }

        public static readonly BindableProperty SpaceProperty = 
            BindableProperty.Create(nameof(Space), typeof(int), typeof(Spacer), defaultValue: 20, propertyChanged: OnSpacePropertyChanged);

        private static void OnSpacePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(bindable is Spacer spacer)
            {
                spacer.Box.HeightRequest = spacer.Space;
            }
        }

        public int Space 
        {
            get => (int)GetValue(SpaceProperty);
            set => SetValue(SpaceProperty, value);
        }
    }

}
