using Xamarin.Forms;

namespace OneMinuteMystery
{
	public partial class OneMinuteMysteryPage : ContentPage
	{
		public OneMinuteMysteryPage()
		{
			InitializeComponent();



			// Your label tap event
			var enter_tap = new TapGestureRecognizer();
			enter_tap.Tapped += (s, e) =>
			{
				Application.Current.MainPage = new MysteryPage();
			};

			lblEnter.GestureRecognizers.Add(enter_tap);
		}
	}
}

