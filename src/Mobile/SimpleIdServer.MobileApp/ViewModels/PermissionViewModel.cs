namespace SimpleIdServer.MobileApp.ViewModels
{
    public class PermissionViewModel : BaseViewModel
    {
        private string _permissionId;
        private string _displayName;
        private bool _isSelected;

        public string PermissionId
        {
            get { return _permissionId; }
            set { SetProperty(ref _permissionId, value); }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
    }
}
