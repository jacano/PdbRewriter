using Android.App;
using Android.Widget;
using Android.OS;
using GoogleAnalyticsTracker.Core;

namespace App2
{
    [Activity(Label = "App2", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            EnumExtensions.IsNullableEnum(typeof(int));

            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }
    }
}

