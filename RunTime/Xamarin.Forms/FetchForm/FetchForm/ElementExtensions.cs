using System;

namespace FetchForm
{
	public class ListView : Xamarin.Forms.ListView
	{
		public ListView () : base()
		{
			ORect = Xamarin.Forms.Rectangle.Zero;
		}

		public Xamarin.Forms.Rectangle ORect {
			get;
			set;
		}

	}

	public class Editor : Xamarin.Forms.Editor
	{
		public Editor () : base()
		{
			ORect = Xamarin.Forms.Rectangle.Zero;
			InputTransparent = false;
			this.IsEnabled = true;
			this.IsVisible = true;
		}

		public Xamarin.Forms.Rectangle ORect {
			get;
			set;
		}

	}

	public class Label : Xamarin.Forms.Label
	{
		public Label () : base()
		{
			ORect = Xamarin.Forms.Rectangle.Zero;
		}

		public Xamarin.Forms.Rectangle ORect {
			get;
			set;
		}

	}

	public class Entry : Xamarin.Forms.Entry
	{
		public Entry () : base()
		{
			ORect = Xamarin.Forms.Rectangle.Zero;
			InputTransparent = false;
			this.IsEnabled = true;
			this.IsVisible = true;
			this.Keyboard = Xamarin.Forms.Keyboard.Text;
		}


		public Xamarin.Forms.Rectangle ORect {
			get;
			set;
		}

	}

	public class Image : Xamarin.Forms.Image
	{
		public Image () : base()
		{
			ORect = Xamarin.Forms.Rectangle.Zero;
		}

		public Xamarin.Forms.Rectangle ORect {
			get;
			set;
		}

	}

	public class ActivityIndicator : Xamarin.Forms.ActivityIndicator
	{
		public ActivityIndicator () : base()
		{
			ORect = Xamarin.Forms.Rectangle.Zero;
		}

		public Xamarin.Forms.Rectangle ORect {
			get;
			set;
		}

	}
		
	public class Stepper : Xamarin.Forms.Stepper
	{
		public Stepper () : base()
		{
			ORect = Xamarin.Forms.Rectangle.Zero;
		}

		public Xamarin.Forms.Rectangle ORect {
			get;
			set;
		}

	}

	public class Slider : Xamarin.Forms.Slider
	{
		public Slider () : base()
		{
			ORect = Xamarin.Forms.Rectangle.Zero;
		}

		public Xamarin.Forms.Rectangle ORect {
			get;
			set;
		}

	}

	public class Button : Xamarin.Forms.Button
	{
		public Button () : base()
		{
			ORect = Xamarin.Forms.Rectangle.Zero;
		}

		public Xamarin.Forms.Rectangle ORect {
			get;
			set;
		}

	}


}

