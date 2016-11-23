package md58c119b09c95a47a065ba97b4cd99195c;


public class TextToSpeechImplementation
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.speech.tts.TextToSpeech.OnInitListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onInit:(I)V:GetOnInit_IHandler:Android.Speech.Tts.TextToSpeech/IOnInitListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("TextToSpeechImplementation, OneMinuteMystery.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", TextToSpeechImplementation.class, __md_methods);
	}


	public TextToSpeechImplementation () throws java.lang.Throwable
	{
		super ();
		if (getClass () == TextToSpeechImplementation.class)
			mono.android.TypeManager.Activate ("TextToSpeechImplementation, OneMinuteMystery.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onInit (int p0)
	{
		n_onInit (p0);
	}

	private native void n_onInit (int p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
