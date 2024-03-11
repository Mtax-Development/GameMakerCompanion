#pragma warning disable IDE0003
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input.Platform;
using Avalonia.Media;
using Avalonia.Layout;
using System.Runtime.InteropServices;
using System;

namespace GameMakerCompanion.Interface
{
    /// <summary> Template from which all interface windows in this application inherit. </summary>
    public class WindowTemplate : Window
    {
        /// <summary> Size in pixels of the window cross exit button contained in its corner. </summary>
        public readonly static PixelSize ExitButtonSize = new(30, (32 + ((Application.OperatingSystem.Platform == OSPlatform.OSX) ? 4 : 0)));

        /// <summary> Handle through which clipboard can be used. </summary>
        /// <remarks> Initialized during creation of the window. </remarks>
        public IClipboard? ClipboardHandle = null;

        /// <summary> Reusable button providing functionality to close the window. </summary>
        internal readonly Button ExitButton = new()
        {
            Content = '✕',
            Foreground = Brushes.White,
            Background = Brushes.Transparent,
            Width = ExitButtonSize.Width,
        };
        
        public WindowTemplate(double width, double height)
        {
            Title = Application.Name;
            Icon = Application.TrayIcon?.Icon;
            Width = width;
            Height = height;
            ClipboardHandle = Clipboard;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SystemDecorations = SystemDecorations.None;
            Background = Brushes.Transparent;
            Color? backgroundColor;

            if (Application.OperatingSystem.Platform == OSPlatform.Windows)
            {
                //|Acrylic blur is only supported on Windows platforms.
                TransparencyLevelHint = new WindowTransparencyLevel[] {WindowTransparencyLevel.AcrylicBlur,
                                                                       WindowTransparencyLevel.Transparent};
                backgroundColor = new Color(80, 185, 20, 130);
            }
            else
            {
                TransparencyLevelHint = new WindowTransparencyLevel[] {WindowTransparencyLevel.Transparent};
                backgroundColor = new Color(240, 115, 20, 85);
            }
            
            Rectangle background = new()
            {
                Width = this.Width,
                Height = this.Height,
                Fill = new SolidColorBrush((Color)backgroundColor),
                Stroke = new SolidColorBrush(new Color(200, 40, 20, 40)),
                StrokeThickness = 2
            };
            
            ExitButton.RenderTransform = ((Application.OperatingSystem.WindowControlAlignment == HorizontalAlignment.Right)
                                          ? new TranslateTransform((Width - ExitButton.Width - background.StrokeThickness), background.StrokeThickness)
                                          : new TranslateTransform(background.StrokeThickness, background.StrokeThickness));
            ExitButton.Click += delegate
            {
                this.Close();
            };
            
            Canvas canvas = new Canvas();
            canvas.Children.Add(background);
            canvas.Children.Add(ExitButton);
            Content = canvas;
        }
        
        /// <summary> Attempt to set the operating system clipboard to the specified text. </summary>
        /// <param name="text"> Text to copy to the clipboard. </param>
        internal void SetClipboard(string text)
        {
            Clipboard?.SetTextAsync(text);
        }
        
        public override void Show()
        {
            Application.PrimaryWindow?.Close();
            Application.PrimaryWindow = this;
            
            base.Show();
        }
    }
}
