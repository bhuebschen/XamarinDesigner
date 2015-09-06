using System;
using Gtk;
using System.Xml;
using Gdk;

namespace FetchForm
{
	public static class Parser
	{
		public static void FetchForm(Container Parent, XmlNode Nodes)
		{
			int cIndex = 0;
			string[] UFont;
			XmlNodeList original = Nodes.ChildNodes;
			XmlNodeList reverse = new ReverseXmlList(original);

			foreach (XmlNode C in reverse) {
				if (C.Name == "Object") {
					cIndex++;
					if (C.Attributes[0].Name == "type") {
						if (C.Attributes["type"].Value.StartsWith("System.Windows.Forms.PictureBox") || C.Attributes["type"].Value.StartsWith("System.Windows.Forms.Button")) {
							var PB = new Gtk.Button();
							global::Gtk.Fixed.FixedChild PBC1 = ((global::Gtk.Fixed.FixedChild)(Parent[PB]));
							Parent.Add(PB);
							foreach (XmlNode V in C.ChildNodes) {
								if (V.Name == "Property") {
									switch (V.Attributes["name"].Value) {
										case "BorderStyle":
											if (V.InnerText == "Solid") {
												//PB.ModifierStyle = 
											}
											break;
										case "Name":
											PB.Name = V.InnerText;
											break;
										case "ForeColor":
											var FColor = V.InnerText.GetXColor().ToNative();
											PB.Children[0].ModifyFg(StateType.Normal, FColor);
											PB.Children[0].ModifyFg(StateType.Active, FColor);
											PB.Children[0].ModifyFg(StateType.Prelight, FColor);
											PB.Children[0].ModifyFg(StateType.Selected, FColor);
											break;
										case "Caption":
										case "Text":
											PB.Label = global::Mono.Unix.Catalog.GetString(System.Environment.ExpandEnvironmentVariables(V.InnerText).Replace("%DATE%", System.DateTime.Now.ToString("dd.MM.yyyy")).Replace("%TIME%", System.DateTime.Now.ToString("hh:mm:ss")));
											break;
										case "BackColor":
											var BColor = V.InnerText.GetXColor().ToNative();
											PB.ModifyBg(StateType.Normal, BColor);
											PB.ModifyBg(StateType.Active, BColor);
											PB.ModifyBg(StateType.Insensitive, BColor);
											PB.ModifyBg(StateType.Prelight, BColor);
											PB.ModifyBg(StateType.Selected, BColor);
											break;
										case "SizeMode":
											//PB.SizeMode = Enum.Parse(typeof(PictureBoxSizeMode), V.InnerText);
											break;
										case "Location":
											PBC1.X = (int)(Convert.ToInt32(V.InnerText.Substring(0, V.InnerText.IndexOf(","))) * App.ScaleFactorX);
											PBC1.Y = (int)(Convert.ToInt32(V.InnerText.Substring(V.InnerText.IndexOf(",") + 1)) * App.ScaleFactorY);
											break;
										case "Size":
											PB.SetSizeRequest((int)(Convert.ToInt32(V.InnerText.Substring(0, V.InnerText.IndexOf(","))) * App.ScaleFactorX), (int)(Convert.ToInt32(V.InnerText.Substring(V.InnerText.IndexOf(",") + 1)) * App.ScaleFactorY));
											break;
										case "Font":
											string VC = V.InnerText;
											UFont = VC.Replace("style=", "").Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
											string VCFName = UFont[0];
											float VCSize = float.Parse(UFont[1].Replace("pt", ""));
											float Z = (float)(VCSize * App.ScaleFactorY);
											PB.Children[0].ModifyFont(Pango.FontDescription.FromString(VCFName + " " + ((int)Z).ToString()));
											break;
										case "Image":
											if (V.HasChildNodes) {
												string IMGData = V.FirstChild.InnerText;
												byte[] B = System.Convert.FromBase64String(IMGData);
												Pixbuf P = new Pixbuf(B);
												P=P.ScaleSimple(PB.WidthRequest-10, PB.HeightRequest, InterpType.Bilinear);
												PB.Image = new Gtk.Image(P);
											}
											break;
									}
								} else if (V.Name == "Object") {
									//FetchForm(PB, V.ParentNode);
								}
							}
						} else if (C.Attributes["type"].Value.StartsWith("System.Windows.Forms.Label")) {
							var CE = new Gtk.EventBox();
							CE.ResizeMode = ResizeMode.Parent;
							var CC = new Gtk.Label();
							CE.Add(CC);
							Parent.Add(CE);
							CC.LineWrapMode = Pango.WrapMode.Word;
							CC.LineWrap = true;
							CC.Wrap = true;
							CC.Justify = Justification.Fill;
							global::Gtk.Fixed.FixedChild PBC1;
							if ((Parent[CE]).GetType().ToString() == "Gtk.Container+ContainerChild") {
								var XVC = Parent[CE].Parent.Parent;
								PBC1 = (global::Gtk.Fixed.FixedChild)(Parent[XVC]);
							} else {
								PBC1 = ((global::Gtk.Fixed.FixedChild)(Parent[CE]));
							}
							foreach (XmlNode V in C.ChildNodes) {
								if (V.Name == "Property") {
									switch (V.Attributes["name"].Value) {
										case "BorderStyle":
											if (V.InnerText == "Solid") {
												//PB.ModifierStyle = 
											}
											break;
										case "Name":
											CC.Name = V.InnerText;
											break;
										case "Font":
											string VC = V.InnerText;
											UFont = VC.Replace("style=", "").Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
											string VCFName = UFont[0];
											float VCSize = float.Parse(UFont[1].Replace("pt", ""));
											float Z = (float)(VCSize * App.ScaleFactorY);
											CC.ModifyFont(Pango.FontDescription.FromString(VCFName + " " + (int)Z));
											break;
										case "ForeColor":
											var FColor = V.InnerText.GetXColor().ToNative();
											CC.ModifyFg(StateType.Normal, FColor);
											CC.ModifyFg(StateType.Active, FColor);
											CC.ModifyFg(StateType.Insensitive, FColor);
											CC.ModifyFg(StateType.Prelight,FColor);
											CC.ModifyFg(StateType.Selected, FColor);
											CE.ModifyFg(StateType.Normal, FColor);
											CE.ModifyFg(StateType.Active, FColor);
											CE.ModifyFg(StateType.Insensitive, FColor);
											CE.ModifyFg(StateType.Prelight, FColor);
											CE.ModifyFg(StateType.Selected, FColor);
											CC.Markup = "<span foreground=\"" + V.InnerText + "\">" + CC.Text + "</span>";
											break;
										case "Caption":
										case "Text":
											CC.Text = global::Mono.Unix.Catalog.GetString(System.Environment.ExpandEnvironmentVariables(V.InnerText).Replace("%DATE%", System.DateTime.Now.ToString("dd.MM.yyyy")).Replace("%TIME%", System.DateTime.Now.ToString("hh:mm:ss")).Replace("##", "\r\n"));
											break;
										case "BackColor":
											if (V.InnerText != "Transparent") {
												var BColor = V.InnerText.GetXColor().ToNative();
												CE.ModifyBg(StateType.Normal, BColor);
												CE.ModifyBg(StateType.Active, BColor);
												CE.ModifyBg(StateType.Insensitive, BColor);
												CE.ModifyBg(StateType.Prelight, BColor);
												CE.ModifyBg(StateType.Selected, BColor);
											} else {
												CE.Visible = false;
												CE.VisibleWindow = false;
											}
											break;
										case "SizeMode":
											//PB.SizeMode = Enum.Parse(typeof(PictureBoxSizeMode), V.InnerText);
											break;
										case "Location":
											PBC1.X = (int)(Convert.ToInt32(V.InnerText.Substring(0, V.InnerText.IndexOf(","))) * App.ScaleFactorX);
											PBC1.Y = (int)(Convert.ToInt32(V.InnerText.Substring(V.InnerText.IndexOf(",") + 1)) * App.ScaleFactorY);
											break;
										case "TextAlign":
											CC.Justify = (V.InnerText == "MiddleCenter" || V.InnerText == "TopCenter" || V.InnerText == "BottomCenter" ? Justification.Center : (V.InnerText == "MiddleRight" || V.InnerText == "TopRight" || V.InnerText == "BottomRight" ? Justification.Right : Justification.Left));
											break;
										case "Size":
											CE.SetSizeRequest((int)(Convert.ToInt32(V.InnerText.Substring(0, V.InnerText.IndexOf(","))) * App.ScaleFactorX), (int)(Convert.ToInt32(V.InnerText.Substring(V.InnerText.IndexOf(",") + 1)) * App.ScaleFactorY));
											CC.SetSizeRequest((int)(Convert.ToInt32(V.InnerText.Substring(0, V.InnerText.IndexOf(","))) * App.ScaleFactorX), (int)(Convert.ToInt32(V.InnerText.Substring(V.InnerText.IndexOf(",") + 1)) * App.ScaleFactorY));
											break;
									}
								} else if (V.Name == "Object") {
									var TZJE = new Fixed();


								}
							}
						} else if (C.Attributes ["type"].Value.StartsWith("System.Windows.Forms.TextBox")) {
							if (C.InnerXml.IndexOf("\"Multiline\">True") > -1)
							{
								var CC = new Gtk.Entry ();
								global::Gtk.Fixed.FixedChild PBC1 = ((global::Gtk.Fixed.FixedChild)(Parent [CC]));
								Parent.Add (CC);
								foreach (XmlNode V in C.ChildNodes) {
									{
										if (V.Name == "Property")
										{
											switch (V.Attributes ["name"].Value)
											{
												case "Text":
													{
														if (V.InnerText.Contains ("%")) {
															CC.Text = System.Environment.ExpandEnvironmentVariables (V.InnerText);
															CC.Position = CC.Text.Length;
														}
														break;
													}
												case "Location":
													PBC1.X = (int)(Convert.ToInt32 (V.InnerText.Substring (0, V.InnerText.IndexOf (",")))  * App.ScaleFactorX);
													PBC1.Y = (int)(Convert.ToInt32 (V.InnerText.Substring (V.InnerText.IndexOf (",") + 1)) * App.ScaleFactorY);
													break;
												case "Size":
													CC.SetSizeRequest ((int)(Convert.ToInt32 (V.InnerText.Substring (0, V.InnerText.IndexOf (",")))* App.ScaleFactorX), (int)(Convert.ToInt32 (V.InnerText.Substring (V.InnerText.IndexOf (",") + 1))* App.ScaleFactorY));
													break;
												case "Name":
													CC.Name = V.InnerText;
													break;
												case "Font":
													string VC = V.InnerText;
													UFont = VC.Replace("style=", "").Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
													string VCFName = UFont[0];
													float VCSize = float.Parse(UFont[1].Replace("pt", ""));
													float Z = (float)(VCSize * App.ScaleFactorY);
													CC.ModifyFont (Pango.FontDescription.FromString (VCFName+" "+((int)Z).ToString()));
													System.Diagnostics.Debug.WriteLine(VCFName+" "+((int)Z).ToString());
													break;
											}
										}
									}
								}
							}
							else
							{
								var CC= new Gtk.Entry();
								global::Gtk.Fixed.FixedChild PBC1 = ((global::Gtk.Fixed.FixedChild)(Parent [CC]));
								Parent.Add (CC);
								foreach (XmlNode V in C.ChildNodes) {
									{
										if (V.Name == "Property")
										{
											switch (V.Attributes ["name"].Value)
											{
												case "Name":
													CC.Name = V.InnerText;
													break;
												case "Text":
													{
														if (V.InnerText.Contains ("%")) {
															CC.Text = System.Environment.ExpandEnvironmentVariables (V.InnerText);
															CC.Position = CC.Text.Length;
														}
														break;
													}
												case "Location":
													PBC1.X = (int)(Convert.ToInt32 (V.InnerText.Substring (0, V.InnerText.IndexOf (",")))  * App.ScaleFactorX);
													PBC1.Y = (int)(Convert.ToInt32 (V.InnerText.Substring (V.InnerText.IndexOf (",") + 1)) * App.ScaleFactorY);
													break;
												case "Size":
													CC.SetSizeRequest ((int)(Convert.ToInt32 (V.InnerText.Substring (0, V.InnerText.IndexOf (",")))* App.ScaleFactorX), (int)(Convert.ToInt32 (V.InnerText.Substring (V.InnerText.IndexOf (",") + 1))* App.ScaleFactorY));
													break;
												case "PasswordChar":
													CC.InvisibleChar = '*';
													CC.Visibility = false;
													break;
												case "Font":
													string VC = V.InnerText;
													UFont = VC.Replace("style=", "").Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
													string VCFName = UFont[0];
													float VCSize = float.Parse(UFont[1].Replace("pt", ""));
													float Z = (float)(VCSize * App.ScaleFactorY);
													CC.ModifyFont (Pango.FontDescription.FromString (VCFName+" "+((int)Z).ToString()));
													System.Diagnostics.Debug.WriteLine(VCFName+" "+((int)Z).ToString());
													break;
											}
										}
									}
								}
							}
						} else if (C.Attributes ["type"].Value.StartsWith ("System.Windows.Forms.Panel")) {
							var CP = new Gtk.Fixed ();
							//CP.Clicked += HandleClick;
							global::Gtk.Fixed.FixedChild PBC1 = ((global::Gtk.Fixed.FixedChild)(Parent [CP]));
							Parent.Add (CP);
							foreach (XmlNode V in C.ChildNodes) {
								if (V.Name == "Property") {
									switch (V.Attributes ["name"].Value) {
										case "Name":
											CP.Name = V.InnerText;
											break;
										case "BorderStyle":
											if (V.InnerText == "Solid") {
												//PB.ModifierStyle = 
											}
											break;
										case "ForeColor":

											CP.Children [0].ModifyFg (StateType.Normal, V.InnerText.GetXColor ().ToNative ());
											break;
										case "BackColor":
											CP.ModifyBg (StateType.Normal, V.InnerText.GetXColor().ToNative ());
											break;
										case "SizeMode":
											//PB.SizeMode = Enum.Parse(typeof(PictureBoxSizeMode), V.InnerText);
											break;
										case "Location":
											PBC1.X = (int)(Convert.ToInt32 (V.InnerText.Substring (0, V.InnerText.IndexOf (",")))  * App.ScaleFactorX);
											PBC1.Y = (int)(Convert.ToInt32 (V.InnerText.Substring (V.InnerText.IndexOf (",") + 1)) * App.ScaleFactorY);
											break;
										case "Size":
											CP.SetSizeRequest ((int)(Convert.ToInt32 (V.InnerText.Substring (0, V.InnerText.IndexOf (",")))* App.ScaleFactorX), (int)(Convert.ToInt32 (V.InnerText.Substring (V.InnerText.IndexOf (",") + 1))* App.ScaleFactorY));
											break;
									}
								} else if (V.Name == "Object") {
									//FetchForm(PB, V.ParentNode);
								}
							}
						} else {
							System.Diagnostics.Debug.WriteLine (C.Attributes ["type"].Value);

						}
					}
				}

			}
			Parent.ShowAll();
		}
	}
}

