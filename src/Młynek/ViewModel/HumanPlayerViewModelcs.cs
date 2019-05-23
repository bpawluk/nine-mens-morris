using Młynek.Utils;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Młynek.ViewModel
{
    class HumanPlayerViewModel : INotifyPropertyChanged
    {
        private ICommand _chooseFieldCmd;
        private bool _inputEnabled;
        private string _message;

        public Action<int> OnFieldChosen;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool InputEnabled
        {
            get => _inputEnabled;
            set
            {
                _inputEnabled = value;
                (_chooseFieldCmd as Command)?.RaiseCanExecuteChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                NotifyPropertyChanged("Message");
            }
        }

        public ICommand ChooseFieldCmd
        {
            get
            {
                if (_chooseFieldCmd == null) _chooseFieldCmd = new Command(ChooseField, parameter => InputEnabled);
                return _chooseFieldCmd;
            }
        }

        public void BeginInteraction(string message, Action<int> callback)
        {
            Message = message;
            OnFieldChosen = callback;
            InputEnabled = true;
        }

        public void EndInteraction()
        {
            InputEnabled = false;
            Message = "";
            OnFieldChosen = null;
        }

        private void ChooseField(object parameter)
        {
            int coords = int.Parse(parameter as string ?? "-1");
            OnFieldChosen?.Invoke(coords);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
