using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FirstApp
{
    public class CardViewCell : ViewCell
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(CardViewCell), null, BindingMode.OneWay, propertyChanged: async (BindableObject bindable, object oldValue, object newValue) =>
            {
                var ctrl = (CardViewCell)bindable;
                ctrl.Text = (string)newValue;
            });

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); textLabel.Text = value; }
        }

        public static readonly BindableProperty DetailProperty =
            BindableProperty.Create(nameof(Detail), typeof(string), typeof(CardViewCell), null, BindingMode.OneWay, propertyChanged: async (BindableObject bindable, object oldValue, object newValue) =>
            {
                var ctrl = (CardViewCell)bindable;
                ctrl.Detail = (string)newValue;
            });

        public string Detail
        {
            get { return (string)GetValue(DetailProperty); }
            set { SetValue(DetailProperty, value); detailTextLabel.Text = value; }
        }

        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(CardViewCell), null, BindingMode.OneWay, propertyChanged: async (BindableObject bindable, object oldValue, object newValue) =>
            {
                var ctrl = (CardViewCell)bindable;
                ctrl.ImageSource = (ImageSource)newValue;
            });

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); image.Source = value; }
        }


        StackLayout layout;
        Image image;
        Label textLabel;
        Label detailTextLabel;

        public CardViewCell()
        {
            image = new Image
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                WidthRequest = 220,
                HeightRequest = 200,
                Source = ImageSource.FromUri(new Uri("https://avatars3.githubusercontent.com/u/1091304?v=3&s=460"))
            };

            textLabel = new Label
            {
                FontSize = Device.OnPlatform<double>(15, 15, 15),
                TextColor = Device.OnPlatform<Color>(Color.FromHex("030303"), Color.FromHex("030303"), Color.Black),
                HorizontalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Start,
                Margin = Device.OnPlatform<Thickness>(new Thickness(12, 10, 12, 4), new Thickness(20, 0, 20, 5), new Thickness(0)),
                LineBreakMode = LineBreakMode.WordWrap,
                VerticalOptions = LayoutOptions.End,
                Text = "Pierce Boggan"
            };

            detailTextLabel = new Label
            {
                FontSize = Device.OnPlatform<double>(13, 13, 13),
                TextColor = Device.OnPlatform<Color>(Color.FromHex("8F8E94"), Color.FromHex("8F8E94"), Color.Black),
                HorizontalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Start,
                Margin = Device.OnPlatform<Thickness>(new Thickness(12, 0, 10, 12), new Thickness(20, 0, 20, 20), new Thickness(0)),
                LineBreakMode = LineBreakMode.WordWrap,
                VerticalOptions = LayoutOptions.End,
                Text = "pierce@xamarin.com"
            };

            layout = new StackLayout
            {
                BackgroundColor = Color.White,
                Spacing = 0,
                Children = { image, textLabel, detailTextLabel }
            };

            View = layout;
        }
    }
}
