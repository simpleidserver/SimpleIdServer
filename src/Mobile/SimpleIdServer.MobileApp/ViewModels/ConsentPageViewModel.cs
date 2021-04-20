using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace SimpleIdServer.MobileApp.ViewModels
{
    public class ConsentPageViewModel : BaseViewModel
    {
        public ConsentPageViewModel()
        {
            Permissions = new ObservableCollection<PermissionViewModel>();
            RejectCommand = new Command(HandleReject, IsRejectEnabled);
            ConfirmCommand = new Command(HandleConfirm, IsConfirmEnabled);
        }

        public string AuthReqId { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public bool IsAnimated { get; set; }
        public ObservableCollection<PermissionViewModel> Permissions { get; set; }
        public event EventHandler IsRejected;
        public event EventHandler IsConfirmed;
        public ICommand RejectCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }

        private void HandleReject()
        {
            if(IsRejected != null)
            {
                IsRejected(this, EventArgs.Empty);
            }
        }

        private bool IsRejectEnabled()
        {
            return true;
        }

        private void HandleConfirm()
        {
            if (IsConfirmed != null)
            {
                IsConfirmed(this, EventArgs.Empty);
            }
        }

        private bool IsConfirmEnabled()
        {
            return true;
        }
    }
}
