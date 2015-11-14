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

namespace EyePad.UI.Talk
{
    /// <summary>
    /// Interaction logic for TalkView.xaml
    /// </summary>
    public partial class TalkView : UserControl
    {
        private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();


        public TalkView()
        {
            InitializeComponent();
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
                await Task.Delay(2000);
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
    }
}
