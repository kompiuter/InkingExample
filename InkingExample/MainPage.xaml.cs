using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InkCanvas.InkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Mouse |
                CoreInputDeviceTypes.Pen |
                CoreInputDeviceTypes.Touch;

            //var drawingAttributes = new InkDrawingAttributes
            //{
            //    Color = Colors.Blue,
            //    Size = new Size(3, 3),
            //    IgnorePressure = false,
            //    FitToCurve = true
            //};

            //InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButtton = sender as RadioButton;
            if (radioButtton?.Content != null)
            {
                switch (radioButtton.Content.ToString())
                {
                    case "Ink":
                        InkCanvas.InkPresenter.InputProcessingConfiguration.Mode =
                            InkInputProcessingMode.Inking;
                        break;
                    case "Erase":
                        InkCanvas.InkPresenter.InputProcessingConfiguration.Mode =
                            InkInputProcessingMode.Erasing;
                        break;
                    default:
                        break;
                }
            }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Don't want to save an empty file
            if (InkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count > 0)
            {
                var savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                savePicker.FileTypeChoices.Add("Gif with embedded ISF", new List<string> { ".gif" });

                var file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await InkCanvas.InkPresenter.StrokeContainer.SaveAsync(stream);
                    }
                }
            }

        }

        private async void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.FileTypeFilter.Add(".isf");
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenSequentialReadAsync())
                {
                    await InkCanvas.InkPresenter.StrokeContainer.LoadAsync(stream);
                }
            }
            
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            InkCanvas.InkPresenter.StrokeContainer.Clear();
        }
    }
}
