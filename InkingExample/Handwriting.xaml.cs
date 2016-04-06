using System;
using System.Text;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
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
    public sealed partial class Handwriting : Page
    {
        public Handwriting()
        {
            this.InitializeComponent();
            this.Loaded += Handwriting_Loaded;
        }

        private void Handwriting_Loaded(object sender, RoutedEventArgs e)
        {
            InkCanvas.InkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Mouse |
                CoreInputDeviceTypes.Pen |
                CoreInputDeviceTypes.Touch;
        }

        private async void Recognize_Click(object sender, RoutedEventArgs e)
        {
            if (InkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count == 0)
            {
                await new MessageDialog("Please enter strokes first").ShowAsync();
                return;
            }

            var inkRecognizerContainer = new InkRecognizerContainer();

            // Find out which recognizers current system has
            var recognizers = inkRecognizerContainer.GetRecognizers();

            var recognitionResults =
                await inkRecognizerContainer.RecognizeAsync(
                    InkCanvas.InkPresenter.StrokeContainer,
                    InkRecognitionTarget.All);

            var recognizedText = new StringBuilder();
            foreach (var inkRecognitionResult in recognitionResults)
            {
                // Add space between words
                if (recognizedText.Length > 0)
                    recognizedText.Append(" ");

                // First is most confident
                recognizedText.Append(inkRecognitionResult.GetTextCandidates()[0]);
            }

            await new MessageDialog(recognizedText.ToString(), "Recognition Results").ShowAsync();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            InkCanvas.InkPresenter.StrokeContainer.Clear();
        }
    }
}
