using System;

using Xamarin.Forms;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace FetchForm
{
	public class App : Application
	{
		public static int ScreenWidth;
		public static int ScreenHeight;
		public static int ContentWidth;
		public static int ContentHeight;
		public static int NavbarHeight = 0;
		public static float ScreenScale = 1;
		public static string OSType = "";
		public static string OSVersion = "1.0";
		public static double ScaleFactorX = 1;
		public static double ScaleFactorY = 1;
		public static object Application;
		public static Page CurrentPage;
		public static NavigationPage Navigation;
		public App instance;

		public App() {
			MainPage = GetMainPage();
		}

		public static Page GetMainPage()
		{
			ContentPage cP = new ContentPage();
			cP.MinimumWidthRequest = ContentWidth;
			cP.MinimumHeightRequest = ContentHeight;
			var absLayout = new AbsoluteLayout();
			absLayout.MinimumWidthRequest = ContentWidth;
			absLayout.MinimumHeightRequest = ContentHeight;
			XDocument newForm = XDocument.Load("example.xdx");
			XElement XForm = ((XElement) newForm.FirstNode);
			App.Navigation = new NavigationPage(cP);
			App.CurrentPage = cP;
			try {
				foreach(XElement U in XForm.Nodes()) {
					if (U.Name == "Property") {
						switch (U.Attribute("name").Value) {
							case "ClientSize":
								{
									string[] PP = ((string)(U.InnerText())).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
									double DisplayWidth = double.Parse(PP[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
									double DisplayHeight = double.Parse(PP[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
									if (DisplayWidth > App.ContentWidth || DisplayWidth < App.ContentWidth) {
										App.ScaleFactorX = App.ContentWidth / DisplayWidth;
									} else if (DisplayWidth < App.ContentWidth) {
										//									App.ScaleFactorX = DisplayWidth / App.ScreenWidth;
									} else {
										App.ScaleFactorX = 1;
									}
									if (DisplayHeight > App.ContentHeight || DisplayWidth < App.ContentHeight) {
										App.ScaleFactorY = App.ContentHeight / DisplayHeight;
									} else if (DisplayWidth < App.ContentHeight) {
										//									App.ScaleFactorY = DisplayHeight / App.ContentHeight;
									} else {
										App.ScaleFactorY = 1;
									}
									//ScaleFactorX = Math.Round(ScaleFactorX,1);
									//ScaleFactorY = Math.Round(ScaleFactorY,1);
									//M = new System.Drawing.RectangleF (new PointF(100,100), new SizeF (Convert.ToInt32 (U.InnerText.Substring(0, U.InnerText.IndexOf(","))), Convert.ToInt32 (U.InnerText.Substring(U.InnerText.IndexOf(",") + 1))));
									break;
								}
							case "Text":
								App.Navigation.Title = U.InnerText();
								cP.Title = U.InnerText();
								break;
							case "ForeColor":
								App.Navigation.BarTextColor = ColorExtensions.GetXColor(String.Concat(U.InnerText()));
								break;
							case "BackColor":
								try {
									absLayout.BackgroundColor = ColorExtensions.GetXColor(U.InnerText());
									App.Navigation.BackgroundColor = absLayout.BackgroundColor;
									App.Navigation.BarBackgroundColor = absLayout.BackgroundColor;
								} catch (Exception ex2) {}
								break;
						}
					}
				}
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
			}
			Parser.FetchForm(absLayout, XForm);
			((ContentPage) App.CurrentPage).Content = absLayout;
			App.CurrentPage = cP;
			cP.Content = absLayout;
			App.CurrentPage = cP;
			return App.Navigation;

		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}

