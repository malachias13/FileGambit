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
        Action CloseAction;
        Action UpdateAction;
        Action<bool> SetAutoUpdates;
        Action SaveData;

        public ICommand CloseCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        private bool _autoShowUpdate;

        public bool AutoShowUpdate
        {
            get { return _autoShowUpdate; }
            set { _autoShowUpdate = value; OnPropertyChanged(); }
        }

        public UpdatePromptViewModel()
        {
            CloseCommand = new RelayCommand(execute => Close(), canExecute => { return true; });
            UpdateCommand = new RelayCommand(execute => Update(), canExecute => { return true; });
        }

        private void Close()
        {
            SetAutoUpdates?.Invoke(_autoShowUpdate);
            SaveData?.Invoke();
            CloseAction?.Invoke();
        }

        private void Update()
        {
            UpdateAction?.Invoke();
            Close();
        }


    }
}
