using PhotoGallery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace File_Gambit.ViewModels
{
    internal class UpdatePromptViewModel : ViewModelBase
    {
        public Action CloseAction;
        public Action UpdateAction;
        public Action<bool> SetAutoUpdates;
        public Action SaveData;

        public ICommand CloseCommand { get; set; }
        public ICommand UpdateCommand { get; set; }

        public string UpdateText
        {
            get { return _updateText; }
            set { _updateText = value; OnPropertyChanged(); } 
        }

        public bool IsButtonEnable
        {
            get { return _isButtonEnable; }
            set { _isButtonEnable = value; OnPropertyChanged(); }
        }

        public bool AutoShowUpdate
        {
            get { return _autoShowUpdate; }
            set { _autoShowUpdate = value; OnPropertyChanged(); }
        }

        private string _updateText;
        private bool _isButtonEnable = true;
        private bool _autoShowUpdate;

        public UpdatePromptViewModel()
        {
            CloseCommand = new RelayCommand(execute => Close(), canExecute => { return true; });
            UpdateCommand = new RelayCommand(execute => Update(), canExecute => { return true; });

            AutoShowUpdate = true;
        }

        private void Close()
        {
            SetAutoUpdates?.Invoke(_autoShowUpdate);
            SaveData?.Invoke();
            CloseAction?.Invoke();
        }

        private void Update()
        {
            IsButtonEnable = false;
            SetAutoUpdates?.Invoke(_autoShowUpdate);
            SaveData?.Invoke();
            UpdateAction?.Invoke();
        }


    }
}
