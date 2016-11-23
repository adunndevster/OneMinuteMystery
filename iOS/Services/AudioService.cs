using System;
using Xamarin.Forms;
using OneMinuteMystery;
using OneMinuteMystery.iOS;
using System.IO;
using Foundation;
using AVFoundation;

[assembly: Dependency(typeof(AudioService))]
namespace OneMinuteMystery.iOS
{
	public class AudioService : IAudio
	{
		public AudioService()
		{
		}

		public void PlayAudioFile(string fileName)
		{
			//string sFilePath = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
			var url = NSUrl.FromString(fileName);
			NSError err;
			var _player = new AVAudioPlayer(url, "mp3", out err); //AVAudioPlayer.FromUrl(url);
			_player.FinishedPlaying += (object sender, AVStatusEventArgs e) =>
			{
				_player = null;
			};
			_player.NumberOfLoops = 0;
			_player.Volume = 1.0f;
			_player.Play();
		}
	}
}