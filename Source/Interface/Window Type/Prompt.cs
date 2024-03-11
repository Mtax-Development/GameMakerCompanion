#pragma warning disable CS8600, CS8602, IDE0003, IDE0047
using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using GameMakerCompanion.Utility;

namespace GameMakerCompanion.Interface.WindowType
{
    /// <summary> Interface window with description and functionality specified upon construction. </summary>
    public class Prompt : WindowTemplate
    {
        internal readonly Button Button = new() {HorizontalContentAlignment = HorizontalAlignment.Center};
        
        /// <param name="text"> Text to display inside the window and to output in the command line. </param>
        /// <param name="buttonText"> Text displayed inside the button in the window. If none, the button is described as an exit button. </param>
        /// <param name="buttonEvent"> Function executed upon clicking the button, in addition to closing this window. </param>
        public Prompt(string text, string? buttonText = null, EventHandler<RoutedEventArgs>? buttonEvent = null) : base(400, 170)
        {
            Console.WriteLine(text);
            
            buttonText ??= UserText.Window.Prompt.DefaultButtonText;
            
            PixelPoint platformOffset = new(0, ((Application.OperatingSystem.Platform == OSPlatform.OSX) ? 4 : 0));
            PixelPoint contentBackgroundOrigin = new((int)Math.Round(Width * 0.02), (ExitButtonSize.Height + 4 - (platformOffset.Y * 2)));
            PixelSize contentBackgroundSize = new((int)(Width - (contentBackgroundOrigin.X * 2)),
                                                  (int)(Height - contentBackgroundOrigin.Y - (contentBackgroundOrigin.X)));
            PixelPoint contentOrigin = new((contentBackgroundOrigin.X + 5), (contentBackgroundOrigin.Y + 5));
            PixelSize contentSize = new((contentBackgroundSize.Width - contentOrigin.X + 3),
                                        (contentBackgroundSize.Height + (contentBackgroundOrigin.X * 2)));
            
            Rectangle contentBackground = new()
            {
                Width = contentBackgroundSize.Width,
                Height = contentBackgroundSize.Height,
                RenderTransform = new TranslateTransform(contentBackgroundOrigin.X, contentBackgroundOrigin.Y),
                Fill = new SolidColorBrush(new Color(90, 0, 0, 0)),
                RadiusX = 5,
                RadiusY = 5
            };
            
            Label title = new()
            {
                Content = new TextBlock()
                {
                    Text = Application.Name,
                    TextWrapping = TextWrapping.NoWrap,
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeight.DemiBold,
                    Width = this.Width,
                    Height = contentBackgroundOrigin.Y,
                    RenderTransform = new TranslateTransform(0, ((int)Math.Round(contentBackgroundOrigin.Y * 0.11) + platformOffset.Y))
                }
            };
            
            Label mainText = new()
            {
                Content = new TextBlock()
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center,
                    Width = (contentSize.Width),
                    Height = (contentSize.Height),
                    RenderTransform = new TranslateTransform(contentOrigin.X - 4, (contentOrigin.Y + (platformOffset.Y * 2))),
                }
            };
            
            Button.Content = buttonText;
            Button.Width = contentSize.Width;
            Button.RenderTransform = new TranslateTransform(contentOrigin.X, (contentBackgroundSize.Height + platformOffset.Y - 4));
            Button.Click += delegate
            {
                this.Close();
            };
            
            if (buttonEvent != null)
            {
                Button.Click += buttonEvent;
            }
            
            Canvas canvas = (Canvas)Content;
            canvas.Children.Add(contentBackground);
            canvas.Children.Add(title);
            canvas.Children.Add(Button);
            canvas.Children.Add(mainText);
        }
    }
}
