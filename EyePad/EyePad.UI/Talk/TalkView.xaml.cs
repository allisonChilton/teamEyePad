using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EyeXFramework.Wpf;

namespace EyePad.UI.Talk
{
    /// <summary>
    /// Interaction logic for TalkView.xaml
    /// </summary>
    public partial class TalkView : UserControl
    {
        private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();

        private TalkViewModel ViewModel
        {
            get { return DataContext as TalkViewModel; }
        }

        public TalkView()
        {
            InitializeComponent();

            _speechSynthesizer.SetOutputToDefaultAudioDevice();
            _speechSynthesizer.Volume = 100;
            _speechSynthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);

            Loaded += TalkView_Loaded;
            Unloaded += TalkView_Unloaded;
        }

        private void TalkView_Loaded(object sender, RoutedEventArgs e)
        {
            CycleThroughAllItemsAsync();
        }

        private void TalkView_Unloaded(object sender, RoutedEventArgs e)
        {
            // Cleanup resources
            _speechSynthesizer.Dispose();
        }


        private void CycleThroughAllItemsAsync()
        {
            CycleThroughKeysAsync();
            Console.WriteLine("Waiting on the key to be selected");
        }

        private async void CycleThroughKeysAsync()
        {
            for(int i = 0; i < KeyboardListBox.Items.Count; i++)
            {
                KeyboardListBox.SelectedIndex = i;
                if (i == KeyboardListBox.Items.Count - 1)
                {
                    i = -1;
                }
                await Task.Delay(1250);
            }
        }

        private void SpeakButton_OnClick(object sender, RoutedEventArgs e)
        {
            _speechSynthesizer.SetOutputToDefaultAudioDevice();
            _speechSynthesizer.Volume = 100;
            _speechSynthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);

            _speechSynthesizer.SpeakAsyncCancelAll();
            _speechSynthesizer.SpeakAsync(OutputTextBox.Text);
            
        }

        private void SpeakButton_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            // TODO - Replace with ICommand implementation
            Button speakButton = sender as Button;
            if (null != speakButton && null != ViewModel)
            {
                bool buttonHasGaze = speakButton.GetHasGaze();
                if (buttonHasGaze)
                {
                    _speechSynthesizer.SpeakAsyncCancelAll();
                    _speechSynthesizer.SpeakAsync(OutputTextBox.Text);
                }
            }
        }

        private void SelectButton_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            // TODO - Replace with ICommand implementation
            Button selectButton = sender as Button;
            if (null != selectButton && null != ViewModel)
            {
                bool buttonHasGaze = selectButton.GetHasGaze();
                if (buttonHasGaze)
                {
                    OutputTextBox.Text += KeyboardListBox.SelectedItem.ToString();
                }
            }
        }

        private void SpaceButton_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            // TODO - Replace with ICommand implementation
            Button spaceButton = sender as Button;
            if (null != spaceButton && null != ViewModel)
            {
                bool buttonHasGaze = spaceButton.GetHasGaze();
                if (buttonHasGaze)
                {
                    OutputTextBox.Text += " ";
                }
            }
        }

        private void BackspaceButton_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            // TODO - Replace with ICommand implementation
            Button backspaceButton = sender as Button;
            if (null != backspaceButton && null != ViewModel)
            {
                bool buttonHasGaze = backspaceButton.GetHasGaze();
                if (buttonHasGaze && OutputTextBox.Text.Length > 0)
                {
                    OutputTextBox.Text = OutputTextBox.Text.Substring(0, OutputTextBox.Text.Length - 1);
                }
            }
        }
    }
}
