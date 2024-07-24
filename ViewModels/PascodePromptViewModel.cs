using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoGallery.ViewModels
{
    internal class PascodePromptViewModel : ViewModelBase
    {
        public string? keycode { get; set; }
        public string TitleTxt {
            get { return _titleText; }
            set
            {
                _titleText = value;
                OnPropertyChanged();
            }
        }
        public ICommand ContinueCommand {  get; set; }
        public ICommand CloseCommand { get; set; }

        private string _titleText;

        public PascodePromptViewModel()
        {

        }

        public bool IsVaildKey()
        {
            return !(keycode is null);
        }

        public void ClearKeyCode()
        {
            keycode = null;
            OnPropertyChanged(nameof(keycode));
        }
    }
}
