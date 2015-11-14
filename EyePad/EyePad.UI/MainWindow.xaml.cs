using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace EyePad.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel
        {
            get { return DataContext as MainWindowViewModel; }
        }

        public MainWindow()
        {
            InitializeComponent();

            // TODO: Remove this when we add an actual menu and/or expand the interface
            ViewModel.Navigate("talk");
        }

        private void MenuButton_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            // TODO - Replace with ICommand implementation
            Button menuButton = sender as Button;
            if (null != menuButton && null != ViewModel)
            {
                bool buttonHasGaze = menuButton.GetHasGaze();
                ViewModel.Navigate("menu");
            }
        }

        private void TalkButton_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            // TODO - Replace with ICommand implementation
            Button talkButton = sender as Button;
            if (null != talkButton && null != ViewModel)
            {
                bool buttonHasGaze = talkButton.GetHasGaze();
                ViewModel.Navigate("talk");
            }
        }
    }
}
