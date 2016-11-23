using System;
using System.Threading.Tasks;
using OneMinuteMystery;

namespace OneMinuteMystery
{
	public interface IBingSpeechApi
	{
		Task<SpeechResult> SpeechToTextAsync(string appId, string key);
		SpeechResult SpeechToText(string appId, string key);
	}
}

