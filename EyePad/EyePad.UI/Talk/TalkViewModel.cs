using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyePad.UI.Talk
{
    class TalkViewModel : BindableBase
    {
        private ObservableCollection<string> _keyboardKeys;

        public ObservableCollection<string> KeyboardKeys
        {
            get { return _keyboardKeys; }
            set { SetProperty(ref _keyboardKeys, value); }
        }

        public TalkViewModel()
        {
            KeyboardKeys = new ObservableCollection<string>()
            {
                "A",
                "B",
                "C",
                "D",
                "E",
                "F",
                "G",
                "H",
                "I",
                "J",
                "K",
                "L",
                "M",
                "N",
                "O",
                "P",
                "Q",
                "R",
                "S",
                "T",
                "U",
                "V",
                "X",
                "Y",
                "Z"
            };
        }
    }
}
