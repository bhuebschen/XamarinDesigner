using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;
using Xamarin.Forms;
using System.Net;
using System.Threading.Tasks;
using FetchForm;

namespace FetchForm
{
	public static class Parser
	{
		public static void FetchForm(AbsoluteLayout Parent, System.Xml.Linq.XNode Nodes) {
			int cIndex = 0;
			int TabIndexes = ((System.Xml.Linq.XElement) Nodes).Elements().Count();
			foreach(XElement C in ((System.Xml.Linq.XElement) Nodes).Elements()) {
				if (C.Name == "Object") {
					cIndex++;
					if (true) {
						if (C.Attribute("type").Value.StartsWith("System.Windows.Forms.PictureBox")) {
							var CP = new Image();
							foreach (XElement V in C.Elements()) {
								if (V.Name == "Property") {
									switch (V.Attribute("name").Value) {
										case "Location":
											CP.ORect = new Rectangle(Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))), Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)), CP.ORect.Width, CP.ORect.Height);
											CP.Layout(new Rectangle(
												Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX,
												Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY,
												CP.Width,
												CP.Height));
											Point pt = new Point(Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX,
												Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY);
											break;
										case "Size":
											CP.ORect = new Rectangle(CP.ORect.Left, CP.ORect.Top, Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))), Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)));
											CP.Layout(new Rectangle(CP.X, CP.Y, Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX, Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY));
											break;
										case "SizeMode":
											if (V.Value == "StretchImage") {
												CP.Aspect = Aspect.Fill ;
											} else if (V.Value == "Zoom") {
												CP.Aspect = Aspect.AspectFill;
											} else if (V.Value == "Normal") {
												CP.Aspect = Aspect.AspectFit;
											}
											break;
										case "Image":
											string IMGData = String.Concat(V.Value);
											byte[] B = System.Convert.FromBase64String(IMGData);
											CP.Source = ImageSource.FromStream(() => new System.IO.MemoryStream(B));
											break;
										case "BackColor":
											CP.BackgroundColor = ColorExtensions.GetXColor(String.Concat(V.Nodes()));
											break;
										case "Tag":
											CP.ClassId = String.Concat(V.Nodes());
											break;
									}
								}
							}
							CP.Aspect = Aspect.AspectFill;
							Parent.Children.Add(CP, new Rectangle(CP.X, CP.Y, CP.Width, CP.Height), AbsoluteLayoutFlags.None);
							Parent.LowerChild(CP);
						} else if (C.Attribute("type").Value.StartsWith("System.Windows.Forms.Button")) {
							var CP = new Button ();
							foreach(XElement V in C.Elements()) {
								if (V.Name == "Property") {
									switch (V.Attribute("name").Value) {
										case "Location":
											CP.ORect = new Rectangle(Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))), Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)), CP.ORect.Width, CP.ORect.Height);
											CP.Layout(new Rectangle(
												Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX,
												Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY,
												CP.Width,
												CP.Height));
											Point pt = new Point(Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX,
												Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY);
											break;
										case "Size":
											CP.ORect = new Rectangle(CP.ORect.Left, CP.ORect.Top, Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))), Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)));
											CP.Layout(new Rectangle(CP.X, CP.Y, Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX, Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY));
											break;
										case "Image":
											string IMGData = String.Concat(V.Value);
											byte[] B = System.Convert.FromBase64String(IMGData);
											var TempFile = System.IO.Path.GetTempFileName();
											System.IO.File.WriteAllBytes(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/" + TempFile, B);
											CP.Image = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/" + TempFile;
											break;
										case "ForeColor":
											CP.TextColor = ColorExtensions.GetXColor(String.Concat(V.Nodes()));
											break;
										case "BackColor":
											CP.BackgroundColor = ColorExtensions.GetXColor(String.Concat(V.Nodes()));
											break;
										case "Caption":
										case "Text":
											CP.Text = String.Concat(V.Nodes()).Replace("##", "\r\n");
											break;
										case "Tag":
											CP.ClassId = String.Concat(V.Nodes());
											break;
										case "Font":
											string[] UFont = String.Concat(V.Nodes()).Replace("style=", "").Replace("Microsoft Sans Serif", "Helvetica").Split(new[] {
												","
											}, StringSplitOptions.RemoveEmptyEntries);
											CP.FontFamily = UFont[0];
											CP.FontSize = float.Parse(UFont[1].Replace("pt", ""));
											if (Array.IndexOf(UFont, " Bold") > -1) {
												CP.FontAttributes = CP.FontAttributes | FontAttributes.Bold;
											}
											if (Array.IndexOf(UFont, " Italic") > -1) {
												CP.FontAttributes = CP.FontAttributes  | FontAttributes.Italic;
											}
											break;
									}
								}
							}
							Parent.Children.Add(CP, new Rectangle(CP.X, CP.Y, CP.Width, CP.Height), AbsoluteLayoutFlags.None);
							Parent.LowerChild(CP);

						} else if (C.Attribute("type").Value.StartsWith("System.Windows.Forms.TextBox")) {
							Point pt = Point.Zero;
							if (C.InnerXml().IndexOf("\"Multiline\">True") > -1) {
								var CP = new Editor();
								foreach(XElement V in C.Elements()) {
									if (V.Name == "Property") {
										switch (V.Attribute("name").Value) {
											case "Tag":
												CP.ClassId = V.InnerText();
												break;
											case "Location":
												CP.ORect = new Rectangle(Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))), Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)), CP.ORect.Width, CP.ORect.Height);
												CP.Layout(new Rectangle(
													Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX,
													Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY,
													CP.Width,
													CP.Height));
												pt = new Point(Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX,
													Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY);
												break;
											case "Size":
												CP.ORect = new Rectangle(CP.ORect.Left, CP.ORect.Top, Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))), Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)));
												CP.Layout(new Rectangle(CP.X, CP.Y, Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX, Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY));
												break;
										}
									}
								}
								CP.InputTransparent = false;
								Parent.Children.Add(CP, new Rectangle(CP.X, CP.Y, CP.Width, CP.Height), AbsoluteLayoutFlags.None);
								Parent.LowerChild(CP);
							} else {
								var CP = new Entry();
								foreach(XElement V in C.Elements()) {
									if (V.Name == "Property") {
										switch (V.Attribute("name").Value) {
											case "Tag":
												CP.ClassId = V.InnerText();
												break;
											case "Location":
												CP.ORect = new Rectangle(Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))), Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)), CP.ORect.Width, CP.ORect.Height);
												CP.Layout(new Rectangle(
													Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX,
													Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY,
													CP.Width,
													CP.Height));
												pt = new Point(Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX,
													Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY);
												break;
											case "Size":
												CP.ORect = new Rectangle(CP.ORect.Left, CP.ORect.Top, Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))), Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)));
												CP.Layout(new Rectangle(CP.X, CP.Y, Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX, Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY));
												break;
										}
									}
								}
								CP.InputTransparent = false;
								Parent.Children.Add(CP, new Rectangle(CP.X, CP.Y, CP.Width, CP.Height), AbsoluteLayoutFlags.None);
								Parent.LowerChild(CP);
							}
							//CP.li
						} else if (C.Attribute("type").Value.StartsWith("System.Windows.Forms.Label")) {
							var CP = new Label();
							foreach(XElement V in C.Elements()) {
								if (V.Name == "Property") {
									switch (V.Attribute("name").Value) {
										case "ForeColor":
											CP.TextColor = ColorExtensions.GetXColor(String.Concat(V.Nodes()));
											break;
										case "BackColor":
											CP.BackgroundColor = ColorExtensions.GetXColor(String.Concat(V.Nodes()));
											break;
										case "Caption":
										case "Text":
											CP.Text = String.Concat(V.Nodes()).Replace("##", "\r\n");
											break;
										case "Tag":
											CP.ClassId = String.Concat(V.Nodes());
											break;
										case "Location":
											CP.ORect = new Rectangle(Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))), Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)), CP.ORect.Width, CP.ORect.Height);
											CP.Layout(new Rectangle(
												Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX,
												Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY,
												CP.Width,
												CP.Height));
											Point pt = new Point(Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX,
												Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY);
											break;
										case "Size":
											CP.ORect = new Rectangle(CP.ORect.Left, CP.ORect.Top, Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))), Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)));
											CP.Layout(new Rectangle(CP.X, CP.Y, Convert.ToInt32(String.Concat(V.Nodes()).Substring(0, String.Concat(V.Nodes()).IndexOf(","))) * App.ScaleFactorX, Convert.ToInt32(String.Concat(V.Nodes()).Substring(String.Concat(V.Nodes()).IndexOf(",") + 1)) * App.ScaleFactorY));
											break;
										case "TabIndex":
											//CP.TabIndex = String.Concat(V.Nodes());
											break;
										case "TextAlign":
											if (String.Concat(V.Nodes()).EndsWith("Center")) {
												CP.XAlign = TextAlignment.Center;
											} else if (String.Concat(V.Nodes()).EndsWith("Right")) {
												CP.XAlign = TextAlignment.End;
											} else {
												CP.XAlign = TextAlignment.Start;
											}
											if (String.Concat(V.Nodes()).StartsWith("Middle")) {
												CP.YAlign = TextAlignment.Center;
											} else if (String.Concat(V.Nodes()).EndsWith("Top")) {
												CP.YAlign = TextAlignment.Start;
											} else {
												CP.YAlign = TextAlignment.End;
											}
											break;
										case "Font":
											string[] UFont = String.Concat(V.Nodes()).Replace("style=", "").Replace("Microsoft Sans Serif", "Helvetica").Split(new[] {
												","
											}, StringSplitOptions.RemoveEmptyEntries);
											CP.FontFamily = UFont[0];
											CP.FontSize = float.Parse(UFont[1].Replace("pt", ""));
											if (Array.IndexOf(UFont, " Bold") > -1) {
												CP.FontAttributes = CP.FontAttributes | FontAttributes.Bold;
											}
											if (Array.IndexOf(UFont, " Italic") > -1) {
												CP.FontAttributes = CP.FontAttributes  | FontAttributes.Italic;
											}
											break;
										case "BorderStyle":
											break;
										case "BorderWidth":
											break;
										case "Enabled":
											CP.IsEnabled = bool.Parse(String.Concat(V.Nodes()));
											break;
									}
								} else if (V.Name == "Object") {
									//FetchForm(CP, V.ParentNode);
								}
							}
							Parent.Children.Add(CP, new Rectangle(CP.X, CP.Y, CP.Width, CP.Height), AbsoluteLayoutFlags.None);
							Parent.LowerChild(CP);
						}
					}
				}
			}
		}
	}
}

