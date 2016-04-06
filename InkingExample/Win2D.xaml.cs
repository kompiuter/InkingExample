using Microsoft.Graphics.Canvas;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

/*
 * All the code found in this project was taken from the MVA
 * course labeled as "Windows 10: Inking and the Ink Canvas"
 * https://mva.microsoft.com/en-US/training-courses/windows-10-inking-and-the-inkcanvas-14586
 */

namespace InkingExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Win2D : Page
    {
        public Win2D()
        {
            this.InitializeComponent();
            var attr = new InkDrawingAttributes
            {
                Color = Colors.Red,
                Size = new Size(4, 4)
            };
            DemoInkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(attr);
            DemoInkCanvas.InkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Mouse |
                CoreInputDeviceTypes.Pen |
                CoreInputDeviceTypes.Touch;
        }

        private async void SaveAnnotatedImage(object sender, RoutedEventArgs e)
        {
            var device = CanvasDevice.GetSharedDevice();

            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(ImageFrame);
            var pixels = (await bitmap.GetPixelsAsync()).ToArray();

            var cBitmap = CanvasBitmap.CreateFromBytes(
                device,
                pixels,
                bitmap.PixelWidth,
                bitmap.PixelHeight,
                Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                DisplayInformation.GetForCurrentView().LogicalDpi);

            var strokes = DemoInkCanvas.InkPresenter.StrokeContainer.GetStrokes();

            var renderTarget = new CanvasRenderTarget(
                device,
                (int)ImageFrame.ActualWidth,
                (int)ImageFrame.ActualHeight,
                96);

            using (var ds = renderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.White);
                ds.DrawImage(cBitmap);
                ds.DrawInk(strokes);
            }

            var saveFile =
                await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "MyAnnotatedImagePng",
                    CreationCollisionOption.ReplaceExisting);

            using (var stream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);
            }
        }

        private async void LoadAnnotatedImage(object sender, RoutedEventArgs e)
        {
            var saveFile =
                await ApplicationData.Current.LocalFolder.GetFileAsync("MyAnnotatedImagePng");

            using (var filestream = await saveFile.OpenAsync(FileAccessMode.Read))
            {
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(filestream);
                SavedImage.Source = bitmapImage;
            }
        }
    }
}
