using System;
using Gtk;
using System.Xml;
using System.Drawing;
using System.Reflection;
using FetchForm;

public partial class MainWindow: Gtk.Window
{


	public MainWindow()
		: base(Gtk.WindowType.Toplevel)
	{
		Build();
		XmlDocument newForm = new XmlDocument();
		newForm.Load("example.xdx");

		XmlNode XForm = newForm.FirstChild;
		RectangleF M = new RectangleF (0, 0, 0, 0);
		try {
			foreach (XmlNode U in XForm.ChildNodes) {
				if (U.Name == "Property") {
					switch (U.Attributes ["name"].Value) {
						case "FormBorderStyle":

							break;
						case "ClientSize":
							{
								M = new System.Drawing.RectangleF (new PointF (100, 100), new SizeF (Convert.ToInt32 (U.InnerText.Substring (0, U.InnerText.IndexOf (","))), Convert.ToInt32 (U.InnerText.Substring (U.InnerText.IndexOf (",") + 1))));
								break;
							}
						case "BackColor":
							try {
								this.ModifyBg (StateType.Normal, U.InnerText.GetXColor().ToNative ());
								//de.SYStemiya.Forms.Window.GetXColor (U.InnerText).ToNative ();
							} catch (Exception ex2) {
							}
							break;
					}
				}
			}

		} catch (Exception ex) {

		}
		//Gdk.Screen.Default.Display.DefaultScreen

		App.ScaleFactorX = (Gdk.Screen.Default.Display.DefaultScreen.Width) / M.Width;
		App.ScaleFactorY = (Gdk.Screen.Default.Display.DefaultScreen.Height) / M.Height;
		App.ScreenWidth = Gdk.Screen.Default.Display.DefaultScreen.Width;
		App.ScreenHeight = Gdk.Screen.Default.Display.DefaultScreen.Height;
		this.DefaultWidth = (int)Gdk.Screen.Default.Display.DefaultScreen.Width;
		this.DefaultHeight = (int)Gdk.Screen.Default.Display.DefaultScreen.Height;
		this.Resize ((int)Gdk.Screen.Default.Display.DefaultScreen.Width, (int)Gdk.Screen.Default.Display.DefaultScreen.Height);
		this.SetSizeRequest ((int)Gdk.Screen.Default.Display.DefaultScreen.Width, (int)Gdk.Screen.Default.Display.DefaultScreen.Height);
		this.Move (0, 0);
		System.Diagnostics.Debug.WriteLine(this.DefaultWidth.ToString() + "x" + this.DefaultHeight.ToString());
		Parser.FetchForm (this.fixed1, XForm);
		this.Child.ShowAll ();

	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}
}
