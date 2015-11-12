using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyePad.UI.Menu;
using EyePad.UI.Talk;

namespace EyePad.UI
{
    class MainWindowViewModel : BindableBase
    {
        // Instantiate each of the ViewModels for the
        // UserControl Views which will be hosted in
        // MainWindow within this ViewModel
        private MenuViewModel _menuViewModel = new MenuViewModel();
        private TalkViewModel _talkViewModel = new TalkViewModel();

        private BindableBase _currentViewModel;
        public BindableBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { SetProperty(ref _currentViewModel, value); }
        }

        public RelayCommand<string> NavigateCommand { get; private set; }

        public MainWindowViewModel()
        {
            NavigateCommand = new RelayCommand<string>(OnNavigate);
        }

        private void OnNavigate(string destination)
        {
            switch (destination)
            {
                case "menu":
                    CurrentViewModel = _menuViewModel;
                    break;
                case "talk":
                    CurrentViewModel = _talkViewModel;
                    break;
                default:
                    CurrentViewModel = _menuViewModel;
                    break;
            }   
        }
    }

}
