using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVM___Form_Validation.ViewModels
{
    class EMP_OLD_ViewModel : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                Name = value;
                OnPropertyChanged();
            }
        }

      

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = "")
        {

            var handler = PropertyChanged; //Sometime it will lose while exexuting 
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }

        }
    }


    class MyCommand : ICommand
    {

        public MyCommand(EMP_OLD_ViewModel emp)
        {

        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
