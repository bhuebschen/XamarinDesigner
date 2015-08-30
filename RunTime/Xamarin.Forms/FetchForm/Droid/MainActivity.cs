using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace FetchForm.Droid
{
	[Activity(Label = "FetchForm.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			App.ScreenWidth = (int)((int)Resources.DisplayMetrics.WidthPixels/Resources.DisplayMetrics.Density); // real pixels
			App.ScreenHeight = (int)((int)Resources.DisplayMetrics.HeightPixels/Resources.DisplayMetrics.Density); // real pixels
			App.ScreenScale = (float)(Resources.DisplayMetrics.ScaledDensity);
			App.ContentWidth = (int)((int)Resources.DisplayMetrics.WidthPixels/Resources.DisplayMetrics.Density); // real pixels
			App.ContentHeight = ((int)((int)Resources.DisplayMetrics.HeightPixels/Resources.DisplayMetrics.Density)-72); // real pixels

			global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(new App());
		}
	}
}

