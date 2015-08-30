using System;
using Xamarin.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace FetchForm
{
	public static class ColorExtensions {
		public static Color FromByteColor(int R, int G, int B) {
			return new Color((float) R / 255.0f, (float) G / 255.0f, (float) B / 255.0f, 255 / 255.0f);
		}
		public static Color FromByteColor(int R, int G, int B, int A) {
			return new Color((float) R / 255.0f, (float) G / 255.0f, (float) B / 255.0f, (float) A / 255.0f);
		}

		public static Color GetXColor(string ColorName) {
			if (ColorName.Contains(",")) {
				string[] C = ColorName.Replace(" ", "").Split(new[] {
					","
				}, StringSplitOptions.RemoveEmptyEntries);
				return new Color(Convert.ToByte(C[0]), Convert.ToByte(C[1]), Convert.ToByte(C[2]));
			} else {
				switch (ColorName.ToLower()) {
					case "transparent":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xff, (int) 0xff, 0);
					case "aliceblue":
						return ColorExtensions.FromByteColor((int) 0xf0, (int) 0xf8, (int) 0xff);
					case "antiquewhite":
						return ColorExtensions.FromByteColor((int) 0xfa, (int) 0xeb, (int) 0xd7);
					case "aqua":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0xff, (int) 0xff);
					case "aquamarine":
						return ColorExtensions.FromByteColor((int) 0x7f, (int) 0xff, (int) 0xd4);
					case "azure":
						return ColorExtensions.FromByteColor((int) 0xf0, (int) 0xff, (int) 0xff);
					case "beige":
						return ColorExtensions.FromByteColor((int) 0xf5, (int) 0xf5, (int) 0xdc);
					case "bisque":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xe4, (int) 0xc4);
					case "black":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0x00, (int) 0x00);
					case "blanchedalmond":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xeb, (int) 0xcd);
					case "blue":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0x00, (int) 0xff);
					case "blueviolet":
						return ColorExtensions.FromByteColor((int) 0x8a, (int) 0x2b, (int) 0xe2);
					case "brown":
						return ColorExtensions.FromByteColor((int) 0xa5, (int) 0x2a, (int) 0x2a);
					case "burlywood":
						return ColorExtensions.FromByteColor((int) 0xde, (int) 0xb8, (int) 0x87);
					case "cadetblue":
						return ColorExtensions.FromByteColor((int) 0x5f, (int) 0x9e, (int) 0xa0);
					case "chartreuse":
						return ColorExtensions.FromByteColor((int) 0x7f, (int) 0xff, (int) 0x00);
					case "chocolate":
						return ColorExtensions.FromByteColor((int) 0xd2, (int) 0x69, (int) 0x1e);
					case "coral":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0x7f, (int) 0x50);
					case "cornflowerblue":
						return ColorExtensions.FromByteColor((int) 0x64, (int) 0x95, (int) 0xed);
					case "cornsilk":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xf8, (int) 0xdc);
					case "crimson":
						return ColorExtensions.FromByteColor((int) 0xdc, (int) 0x14, (int) 0x3c);
					case "cyan":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0xff, (int) 0xff);
					case "darkblue":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0x00, (int) 0x8b);
					case "darkcyan":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0x8b, (int) 0x8b);
					case "darkgoldenrod":
						return ColorExtensions.FromByteColor((int) 0xb8, (int) 0x86, (int) 0x0b);
					case "darkgray":
						return ColorExtensions.FromByteColor((int) 0xa9, (int) 0xa9, (int) 0xa9);
					case "darkgreen":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0x64, (int) 0x00);
					case "darkkhaki":
						return ColorExtensions.FromByteColor((int) 0xbd, (int) 0xb7, (int) 0x6b);
					case "darkmagenta":
						return ColorExtensions.FromByteColor((int) 0x8b, (int) 0x00, (int) 0x8b);
					case "darkolivegreen":
						return ColorExtensions.FromByteColor((int) 0x55, (int) 0x6b, (int) 0x2f);
					case "darkorange":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0x8c, (int) 0x00);
					case "darkorchid":
						return ColorExtensions.FromByteColor((int) 0x99, (int) 0x32, (int) 0xcc);
					case "darkred":
						return ColorExtensions.FromByteColor((int) 0x8b, (int) 0x00, (int) 0x00);
					case "darksalmon":
						return ColorExtensions.FromByteColor((int) 0xe9, (int) 0x96, (int) 0x7a);
					case "darkseagreen":
						return ColorExtensions.FromByteColor((int) 0x8f, (int) 0xbc, (int) 0x8b);
					case "darkslateblue":
						return ColorExtensions.FromByteColor((int) 0x48, (int) 0x3d, (int) 0x8b);
					case "darkslategray":
						return ColorExtensions.FromByteColor((int) 0x2f, (int) 0x4f, (int) 0x4f);
					case "darkturquoise":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0xce, (int) 0xd1);
					case "darkviolet":
						return ColorExtensions.FromByteColor((int) 0x94, (int) 0x00, (int) 0xd3);
					case "deeppink":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0x14, (int) 0x93);
					case "deepskyblue":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0xbf, (int) 0xff);
					case "dimgray":
						return ColorExtensions.FromByteColor((int) 0x69, (int) 0x69, (int) 0x69);
					case "dodgerblue":
						return ColorExtensions.FromByteColor((int) 0x1e, (int) 0x90, (int) 0xff);
					case "firebrick":
						return ColorExtensions.FromByteColor((int) 0xb2, (int) 0x22, (int) 0x22);
					case "floralwhite":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xfa, (int) 0xf0);
					case "forestgreen":
						return ColorExtensions.FromByteColor((int) 0x22, (int) 0x8b, (int) 0x22);
					case "fuchsia":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0x00, (int) 0xff);
					case "gainsboro":
						return ColorExtensions.FromByteColor((int) 0xdc, (int) 0xdc, (int) 0xdc);
					case "ghostwhite":
						return ColorExtensions.FromByteColor((int) 0xf8, (int) 0xf8, (int) 0xff);
					case "gold":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xd7, (int) 0x00);
					case "goldenrod":
						return ColorExtensions.FromByteColor((int) 0xda, (int) 0xa5, (int) 0x20);
					case "gray":
						return ColorExtensions.FromByteColor((int) 0x80, (int) 0x80, (int) 0x80);
					case "green":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0x80, (int) 0x00);
					case "greenyellow":
						return ColorExtensions.FromByteColor((int) 0xad, (int) 0xff, (int) 0x2f);
					case "honeydew":
						return ColorExtensions.FromByteColor((int) 0xf0, (int) 0xff, (int) 0xf0);
					case "hotpink":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0x69, (int) 0xb4);
					case "indianred":
						return ColorExtensions.FromByteColor((int) 0xcd, (int) 0x5c, (int) 0x5c);
					case "indigo":
						return ColorExtensions.FromByteColor((int) 0x4b, (int) 0x00, (int) 0x82);
					case "ivory":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xff, (int) 0xf0);
					case "khaki":
						return ColorExtensions.FromByteColor((int) 0xf0, (int) 0xe6, (int) 0x8c);
					case "lavender":
						return ColorExtensions.FromByteColor((int) 0xe6, (int) 0xe6, (int) 0xfa);
					case "lavenderblush":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xf0, (int) 0xf5);
					case "lawngreen":
						return ColorExtensions.FromByteColor((int) 0x7c, (int) 0xfc, (int) 0x00);
					case "lemonchiffon":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xfa, (int) 0xcd);
					case "lightblue":
						return ColorExtensions.FromByteColor((int) 0xad, (int) 0xd8, (int) 0xe6);
					case "lightcoral":
						return ColorExtensions.FromByteColor((int) 0xf0, (int) 0x80, (int) 0x80);
					case "lightcyan":
						return ColorExtensions.FromByteColor((int) 0xe0, (int) 0xff, (int) 0xff);
					case "lightgoldenrodyellow":
						return ColorExtensions.FromByteColor((int) 0xfa, (int) 0xfa, (int) 0xd2);
					case "lightgray":
						return ColorExtensions.FromByteColor((int) 0xd3, (int) 0xd3, (int) 0xd3);
					case "lightgreen":
						return ColorExtensions.FromByteColor((int) 0x90, (int) 0xee, (int) 0x90);
					case "lightpink":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xb6, (int) 0xc1);
					case "lightsalmon":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xa0, (int) 0x7a);
					case "lightseagreen":
						return ColorExtensions.FromByteColor((int) 0x20, (int) 0xb2, (int) 0xaa);
					case "lightskyblue":
						return ColorExtensions.FromByteColor((int) 0x87, (int) 0xce, (int) 0xfa);
					case "lightslategray":
						return ColorExtensions.FromByteColor((int) 0x77, (int) 0x88, (int) 0x99);
					case "lightsteelblue":
						return ColorExtensions.FromByteColor((int) 0xb0, (int) 0xc4, (int) 0xde);
					case "lightyellow":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xff, (int) 0xe0);
					case "lime":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0xff, (int) 0x00);
					case "limegreen":
						return ColorExtensions.FromByteColor((int) 0x32, (int) 0xcd, (int) 0x32);
					case "linen":
						return ColorExtensions.FromByteColor((int) 0xfa, (int) 0xf0, (int) 0xe6);
					case "magenta":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0x00, (int) 0xff);
					case "maroon":
						return ColorExtensions.FromByteColor((int) 0x80, (int) 0x00, (int) 0x00);
					case "mediumaquamarin":
						return ColorExtensions.FromByteColor((int) 0x66, (int) 0xcd, (int) 0xaa);
					case "mediumblue":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0x00, (int) 0xcd);
					case "mediumorchid":
						return ColorExtensions.FromByteColor((int) 0xba, (int) 0x55, (int) 0xd3);
					case "mediumpurple":
						return ColorExtensions.FromByteColor((int) 0x93, (int) 0x70, (int) 0xdb);
					case "mediumseagreen":
						return ColorExtensions.FromByteColor((int) 0x3c, (int) 0xb3, (int) 0x71);
					case "mediumslateblue":
						return ColorExtensions.FromByteColor((int) 0x7b, (int) 0x68, (int) 0xee);
					case "mediumspringgreen":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0xfa, (int) 0x9a);
					case "mediumturquoise":
						return ColorExtensions.FromByteColor((int) 0x48, (int) 0xd1, (int) 0xcc);
					case "mediumvioletred":
						return ColorExtensions.FromByteColor((int) 0xc7, (int) 0x15, (int) 0x85);
					case "midnightblue":
						return ColorExtensions.FromByteColor((int) 0x19, (int) 0x19, (int) 0x70);
					case "mintcream":
						return ColorExtensions.FromByteColor((int) 0xf5, (int) 0xff, (int) 0xfa);
					case "mistyrose":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xe4, (int) 0xe1);
					case "moccasin":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xe4, (int) 0xb5);
					case "navajowhite":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xde, (int) 0xad);
					case "navy":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0x00, (int) 0x80);
					case "oldlace":
						return ColorExtensions.FromByteColor((int) 0xfd, (int) 0xf5, (int) 0xe6);
					case "olive":
						return ColorExtensions.FromByteColor((int) 0x80, (int) 0x80, (int) 0x00);
					case "olivedrab":
						return ColorExtensions.FromByteColor((int) 0x6b, (int) 0x8e, (int) 0x23);
					case "orange":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xa5, (int) 0x00);
					case "orangered":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0x45, (int) 0x00);
					case "orchid":
						return ColorExtensions.FromByteColor((int) 0xda, (int) 0x70, (int) 0xd6);
					case "palegoldenrod":
						return ColorExtensions.FromByteColor((int) 0xee, (int) 0xe8, (int) 0xaa);
					case "palegreen":
						return ColorExtensions.FromByteColor((int) 0x98, (int) 0xfb, (int) 0x98);
					case "paleturquoise":
						return ColorExtensions.FromByteColor((int) 0xaf, (int) 0xee, (int) 0xee);
					case "palevioletred":
						return ColorExtensions.FromByteColor((int) 0xdb, (int) 0x70, (int) 0x93);
					case "papayawhip":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xef, (int) 0xd5);
					case "peachpuff":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xda, (int) 0xb9);
					case "peru":
						return ColorExtensions.FromByteColor((int) 0xcd, (int) 0x85, (int) 0x3f);
					case "pink":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xc0, (int) 0xcb);
					case "plum":
						return ColorExtensions.FromByteColor((int) 0xdd, (int) 0xa0, (int) 0xdd);
					case "powderblue":
						return ColorExtensions.FromByteColor((int) 0xb0, (int) 0xe0, (int) 0xe6);
					case "purple":
						return ColorExtensions.FromByteColor((int) 0x80, (int) 0x00, (int) 0x80);
					case "red":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0x00, (int) 0x00);
					case "rosybrown":
						return ColorExtensions.FromByteColor((int) 0xbc, (int) 0x8f, (int) 0x8f);
					case "royalblue":
						return ColorExtensions.FromByteColor((int) 0x41, (int) 0x69, (int) 0xe1);
					case "saddlebrown":
						return ColorExtensions.FromByteColor((int) 0x8b, (int) 0x45, (int) 0x13);
					case "salmon":
						return ColorExtensions.FromByteColor((int) 0xfa, (int) 0x80, (int) 0x72);
					case "sandybrown":
						return ColorExtensions.FromByteColor((int) 0xf4, (int) 0xa4, (int) 0x60);
					case "seagreen":
						return ColorExtensions.FromByteColor((int) 0x2e, (int) 0x8b, (int) 0x57);
					case "seashell":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xf5, (int) 0xee);
					case "sienna":
						return ColorExtensions.FromByteColor((int) 0xa0, (int) 0x52, (int) 0x2d);
					case "silver":
						return ColorExtensions.FromByteColor((int) 0xc0, (int) 0xc0, (int) 0xc0);
					case "skyblue":
						return ColorExtensions.FromByteColor((int) 0x87, (int) 0xce, (int) 0xeb);
					case "slateblue":
						return ColorExtensions.FromByteColor((int) 0x6a, (int) 0x5a, (int) 0xcd);
					case "slategray":
						return ColorExtensions.FromByteColor((int) 0x70, (int) 0x80, (int) 0x90);
					case "snow":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xfa, (int) 0xfa);
					case "springgreen":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0xff, (int) 0x7f);
					case "steelblue":
						return ColorExtensions.FromByteColor((int) 0x46, (int) 0x82, (int) 0xb4);
					case "tan":
						return ColorExtensions.FromByteColor((int) 0xd2, (int) 0xb4, (int) 0x8c);
					case "teal":
						return ColorExtensions.FromByteColor((int) 0x00, (int) 0x80, (int) 0x80);
					case "thistle":
						return ColorExtensions.FromByteColor((int) 0xd8, (int) 0xbf, (int) 0xd8);
					case "tomato":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0x63, (int) 0x47);
					case "turquoise":
						return ColorExtensions.FromByteColor((int) 0x40, (int) 0xe0, (int) 0xd0);
					case "violet":
						return ColorExtensions.FromByteColor((int) 0xee, (int) 0x82, (int) 0xee);
					case "wheat":
						return ColorExtensions.FromByteColor((int) 0xf5, (int) 0xde, (int) 0xb3);
					case "white":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xff, (int) 0xff);
					case "whitesmoke":
						return ColorExtensions.FromByteColor((int) 0xf5, (int) 0xf5, (int) 0xf5);
					case "yellow":
						return ColorExtensions.FromByteColor((int) 0xff, (int) 0xff, (int) 0x00);
					case "yellowgreen":
						return ColorExtensions.FromByteColor((int) 0x9a, (int) 0xcd, (int) 0x32);
					default:
						return ColorExtensions.FromByteColor(0, 0, 0);
				}


			}
		}
	}

	public static class XElementExtensiosn {
		public static string InnerXml(this XElement element) {
			return element.Nodes().Aggregate("", (b, node) => b += node.ToString());
		}
		public static string InnerText(this XElement element) {
			return String.Concat(element.Nodes());
		}
	}
}

