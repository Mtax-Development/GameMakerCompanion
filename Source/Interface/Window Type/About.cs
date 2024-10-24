#pragma warning disable CS8600, CS8602, IDE0003, IDE0047
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GameMakerCompanion.Utility;
using Path = GameMakerCompanion.Utility.Path;

namespace GameMakerCompanion.Interface.WindowType
{
    /// <summary> Interface window containing information identifying and describing this application. </summary>
    internal class About : WindowTemplate
    {
        internal About() : base(470, 400)
        {
            if (Application.OperatingSystem.Platform == OSPlatform.Linux)
            {
                FontSize = 14;
            }
            
            Bitmap logoBitmap = new(AssetLoader.Open(Path.File.Logo));
            double logoOriginY = Math.Round(Height * 0.015);
            int logoOffsetY = (int)Math.Round(logoBitmap.Size.Height + logoOriginY + (Height * 0.02));
            
            Image logo = new()
            {
                Source = logoBitmap,
                RenderTransform = new TranslateTransform(((this.Width - logoBitmap.Size.Width) * 0.5), logoOriginY)
            };
            
            PixelPoint contentBackgroundOrigin = new((int)Math.Round(Width * 0.02), logoOffsetY);
            PixelPoint contentOffset = new((int)Math.Round((Width * 0.01)),
                                           (int)Math.Round((Height * 0.01)));
            PixelSize contentSize = new((int)Math.Round((Width * 0.96)), ((int)Math.Round(Height * 0.98) - logoOffsetY));
            PixelPoint contentOrigin = new((contentBackgroundOrigin.X + contentOffset.X),
                                           (logoOffsetY + contentOffset.Y));
            PixelPoint contentEnd = new((contentOrigin.X + contentSize.Width), (contentOrigin.Y + contentSize.Height));
            
            Rectangle contentBackground = new()
            {
                Width = contentSize.Width,
                Height = contentSize.Height,
                RenderTransform = new TranslateTransform(contentBackgroundOrigin.X, contentBackgroundOrigin.Y),
                Fill = new SolidColorBrush(new Color(90, 0, 0, 0)),
                RadiusX = 5,
                RadiusY = 5
            };
            
            PixelPoint bottomButtonOrigin = new((int)Math.Round(contentOffset.X * 0.5), (contentEnd.Y - 50));
            int buttonWidth = 217;
            
            Button openApplicationDirectoryButton = new()
            {
                Width = buttonWidth,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = UserText.Window.About.Button.OpenApplicationDirectory,
                RenderTransform = new TranslateTransform((contentOrigin.X + bottomButtonOrigin.X), bottomButtonOrigin.Y)
            };
            
            openApplicationDirectoryButton.Click += delegate
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = Path.Directory.ApplicationRoot,
                    UseShellExecute = true,
                    Verb = "open"
                });
                
                this.Close();
            };
            
            Button openApplicationRepositoryButton = new()
            {
                Width = buttonWidth,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = UserText.Window.About.Button.OpenApplicationRepository,
                RenderTransform = new TranslateTransform((contentEnd.X - bottomButtonOrigin.X - buttonWidth - 10),
                                                         bottomButtonOrigin.Y)
            };
            
            openApplicationRepositoryButton.Click += delegate
            {
                Application.OperatingSystem.OpenURL(Path.Remote.ProjectRepository);
                this.Close();
            };
            
            Label about = new()
            {
                Content = new TextBlock()
                {
                    Text = UserText.Window.About.Label.ApplicationDescription,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    Width = (contentBackground.Width - contentOrigin.X),
                    Height = (contentOrigin.Y + contentSize.Height),
                    RenderTransform = new TranslateTransform(contentOrigin.X, contentOrigin.Y),
                }
            };
            
            Canvas canvas = (Canvas)Content;
            canvas.Children.Add(contentBackground);
            canvas.Children.Add(logo);
            canvas.Children.Add(about);
            canvas.Children.Add(openApplicationDirectoryButton);
            canvas.Children.Add(openApplicationRepositoryButton);
        }
    }
}
