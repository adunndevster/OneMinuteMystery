using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.ServiceModel.Channels;

namespace OneMinuteMystery
{
	public partial class MysteryPage : ContentPage
	{
		public MysteryPage()
		{
			InitializeComponent();

			BindingContext = new MysteryPageViewModel();
		}
	}
}

