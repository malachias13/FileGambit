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
        public Action<bool> SetAutoUpdatePopup;
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
            get { return _autoShowUpdatePopup; }
            set { _autoShowUpdatePopup = value; OnPropertyChanged(); }
        }

        private string _updateText;
        private bool _isButtonEnable;
        private bool _autoShowUpdatePopup;

        public UpdatePromptViewModel()
        {
            CloseCommand = new RelayCommand(execute => Close(), canExecute => { return _isButtonEnable; });
            UpdateCommand = new RelayCommand(execute => Update(), canExecute => { return _isButtonEnable; });

            AutoShowUpdate = true;
            IsButtonEnable = true;
        }

        private void Close()
        {
            SetAutoUpdatePopup?.Invoke(_autoShowUpdatePopup);
            SaveData?.Invoke();
            CloseAction?.Invoke();
        }

        private void Update()
        {
            IsButtonEnable = false;

            SetAutoUpdatePopup?.Invoke(_autoShowUpdatePopup);
            SaveData?.Invoke();
            UpdateAction?.Invoke();
        }


    }
}
