using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVVM___Form_Validation.ViewModels
{
    class EmployeeViewModel : BindableBase
    {
        #region prop set

        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private int _mobile;

        public int Mobile
        {
            get { return _mobile; }
            set { SetProperty(ref _mobile, value); }
        }

        private double _salray;

        public double Salary
        {
            get { return _salray; }
            set { SetProperty(ref _salray, value); }
        }


        #endregion

        public DelegateCommand Employee_button { get; set; }


        public EmployeeViewModel()
        {
            Employee_button = new DelegateCommand(Exceute, CanExecute).ObservesProperty(() => Name).ObservesProperty(() => Mobile).ObservesProperty(() => Email);
        }

        private bool CanExecute()
        {
            bool _rval = false;
            
            if (String.IsNullOrEmpty(Name))
            {
                 return false;
            }
            
            if(!string.IsNullOrEmpty(Email) && IsValidEmail(Email) == false)
            {
                MessageBox.Show("Please enter valid mail.");
                return _rval = false;                
            }

            if (Mobile.ToString().Trim() == "0" )
            {
                return _rval = false;
            }


            return _rval  = true;
        }

        private void Exceute()
        {

            MessageBox.Show("Verified all Fields.");

        }

        //email validation 
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
