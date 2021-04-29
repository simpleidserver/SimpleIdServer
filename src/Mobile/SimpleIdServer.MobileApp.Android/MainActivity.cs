using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Firebase;
using SimpleIdServer.MobileApp.Droid.Services;
using SimpleIdServer.MobileApp.Services;
using Xamarin.Forms;

namespace SimpleIdServer.MobileApp.Droid
{
    [Activity(
        Label = "SimpleIdServer.MobileApp", 
        Icon = "@mipmap/icon", 
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            FirebaseApp.InitializeApp(this.ApplicationContext);
            TabLayoutResource = Resource.Layout.Tabbar;

            base.OnCreate(savedInstanceState);

            RegisterDependencies();

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            NotifyCallback();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void NotifyCallback()
        {
            if (Intent != null && AndroidLoginProvider.Instance != null)
            {
                AndroidLoginProvider.Instance.NotifyOfCallback(Intent);
            }
        }

        private void RegisterDependencies()
        {
            var notification = new AndroidNotificationManager();
            var androidLoginProvider = new AndroidLoginProvider();
            DependencyService.RegisterSingleton<INotificationManager>(notification);
            DependencyService.RegisterSingleton<ILoginProvider>(androidLoginProvider);
        }
    }
}