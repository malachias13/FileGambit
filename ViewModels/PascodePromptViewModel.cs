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
        public ICommand ContinueCommand {  get; set; }

        public PascodePromptViewModel()
        {

        }

        public bool IsVaildKey()
        {
            return !(keycode is null);
        }
    }
}
