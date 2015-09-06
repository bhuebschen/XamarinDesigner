using System;
using System.Drawing;
using System.Xml;

namespace FetchForm
{
	public static class App
	{
		public static int ScreenWidth;
		public static int ScreenHeight;
		public static int ContentWidth;
		public static int ContentHeight;
		public static float ScreenScale = 1;
		public static string OSType = "";
		public static string OSVersion = "1.0";
		public static double ScaleFactorX = 1;
		public static double ScaleFactorY = 1;
	}

	public static class ColorExtensions
	{
		public static Gdk.Color ToNative(this System.Drawing.Color This)
		{

			return new Gdk.Color(This.R, This.G , This.B);
		}

		public static System.Drawing.Color FromNative(this Gdk.Color This)
		{
			float r,g,b,a;
			return System.Drawing.Color.FromArgb (This.Red, This.Green, This.Blue);
		}

		public static System.Drawing.Color GetXColor(this string ColorName)
		{
			if (ColorName.Contains(",")) {
				string[] C = ColorName.Replace( " ", "").Split(new [] {","}, StringSplitOptions.RemoveEmptyEntries);
				return System.Drawing.Color.FromArgb(255, Convert.ToByte(C[0]), Convert.ToByte(C[1]), Convert.ToByte(C[2]));
			} else {
				System.Drawing.Color C2 = default(System.Drawing.Color);
				C2 = System.Drawing.Color.FromName(ColorName);
				if (C2.IsEmpty) {
					System.Diagnostics.Debugger.Break();
				}
				return C2;
			}
		}

	}

	public class ReverseXmlList : XmlNodeList {
		private readonly XmlNodeList _source;
		public ReverseXmlList(XmlNodeList source)
		{
			_source = source;
		}
		public override XmlNode Item(int index)
		{
			return _source.Item(Count - (index + 1));
		}
		public override System.Collections.IEnumerator GetEnumerator()
		{
			for (int i = Count - 1; i >= 0; i--)
			{
				yield return _source.Item(i);
			}
		}
		public override int Count
		{
			get { return _source.Count; }
		}
	}
}

