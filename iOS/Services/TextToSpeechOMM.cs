using AVFoundation;
using OneMinuteMystery;

[assembly: Xamarin.Forms.Dependency(typeof(TextToSpeechOMM))]
public class TextToSpeechOMM : ITextToSpeech
{
	public TextToSpeechOMM() { }


	public void Speak(string text)
	{
		var speechSynthesizer = new AVSpeechSynthesizer();

		var speechUtterance = new AVSpeechUtterance(text)
		{
			Rate = AVSpeechUtterance.MaximumSpeechRate / 2.0f,
			Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
			Volume = 0.5f,
			PitchMultiplier = 0.62f
		};

		speechSynthesizer.SpeakUtterance(speechUtterance);

	}
}