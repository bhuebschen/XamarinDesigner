using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace FetchForm.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			App.Application = app;
			App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;
			App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
			App.ScreenScale = (float)UIScreen.MainScreen.Scale;
			App.ContentWidth = (int)UIScreen.MainScreen.Bounds.Width;
			nfloat tSize = UIScreen.MainScreen.Bounds.Height - (64);
			App.NavbarHeight = 64;
			App.ContentHeight = (int)tSize;
			App.OSVersion = System.Environment.OSVersion.VersionString;
			App.OSType = "iOS";

			global::Xamarin.Forms.Forms.Init();

			// Code for starting up the Xamarin Test Cloud Agent
			#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
			#endif

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}

