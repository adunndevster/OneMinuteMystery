using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace OneMinuteMystery
{
	public class MysteryPageViewModel : BaseViewModel
	{

		public enum MysteryMode
		{
			Welcome,
			Mystery
		}
		MysteryMode mode = MysteryMode.Welcome;

		public Knowledge knowledge;
		private string textToSay = "";

		public MysteryPageViewModel()
		{
			SetupMystery();
		}


		//}
		bool isTalking = false;
		public bool IsTalking
		{
			get { return isTalking; }
			set { isTalking = value; OnPropertyChanged("TalkButtonText"); }
		}

		public string TalkButtonText
		{
			get
			{
				if (IsTalking)
					return "Stop Talking";
				else
					return "Talk";
			}
		}

		private string responseText = "Mystery Page";
		public string ResponseText
		{
			get
			{
				return responseText;
			}
			set {
				responseText = value;
				OnPropertyChanged("ResponseText");
			}
		}


		Command recordAudioCommand;
		public Command RecordAudioCommand
		{
			get { return recordAudioCommand ?? (recordAudioCommand = new Command(async () => await ExecuteRecordAudioCommandAsync())); }
		}

		async Task ExecuteRecordAudioCommandAsync()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				if (!IsTalking)
				{
					DependencyService.Get<IAudioRecorderService>().StartRecording();

					IsTalking = !IsTalking;
				}
				else
				{
					DependencyService.Get<IAudioRecorderService>().StopRecording();

					IsTalking = !IsTalking;

					////UserDialogs.Instance.ShowLoading("Converting Speech to Text");
					var speech = await DependencyService.Get<IBingSpeechApi>().SpeechToTextAsync("speech", "26d14e37e66f4273ace22c99400e3184");

					HandleSpeech(speech);
				}
			}
			catch (Exception ex)
			{
				///Acr.UserDialogs.UserDialogs.Instance.ShowError(ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}

		void HandleSpeech(SpeechResult speech)
		{

			ResponseText = speech.Name;
			if (mode == MysteryMode.Welcome)
			{
				HandleWelcomeSpeech(speech);

			}
			else if (mode == MysteryMode.Mystery)
			{
				HandleMysterySpeech(speech);
			}
		}

		void HandleWelcomeSpeech(SpeechResult speech)
		{
			var text = speech.Name.ToLower();

			//if (text.Contains("get started") ||
			//   text.Contains("give me a puzzle") ||
			//   text.Contains("give me a mystery") ||
			//   text.Contains("ready")
			//  )
			if(speech.TopIntent.Intent == Intents.START_MYSTERY)
			{
				mode = MysteryMode.Mystery;

				//get started and give the puzzle.
				DependencyService.Get<ITextToSpeech>().Speak(VoiceLines.PUZZLE);

			}
			else {
				DependencyService.Get<ITextToSpeech>().Speak(VoiceLines.REPHRASE);
				//I'm sorry, come again?
			}
		}

		void HandleMysterySpeech(SpeechResult speech)
		{
			///"what is" = "was it" :/. MSFT, get it together man!

			var text = speech.Name.ToLower();


			if (text.Contains(" you "))
			{
				//I have nothing to do with this...
				DependencyService.Get<ITextToSpeech>().Speak(VoiceLines.NOTHING_TO_DO);
			}

			//do they need a clue?
			if (speech.TopIntent.Intent == Intents.CLUE)
			{
				//give one if there is one.
				if(!knowledge.Clues.ElementAt(0).Revealed)
				{
					DependencyService.Get<ITextToSpeech>().Speak(VoiceLines.CLUE1);
					knowledge.Clues.ElementAt(0).Revealed = true;
				} else {
					DependencyService.Get<ITextToSpeech>().Speak(VoiceLines.CLUE2);
					knowledge.Clues.ElementAt(1).Revealed = true;
				}
			}

			//Are they ready for the answer?
			if (speech.TopIntent.Intent == Intents.GIVE_ANSWER)
			{
			
				DependencyService.Get<ITextToSpeech>().Speak(VoiceLines.SOLUTION);
			}

			//let's see if we can determine forced yes/no questions...
			//if (text.Contains("was it") ||
			//   text.Contains("is ") ||
			//    text.StartsWith("was ") ||
			//    text.StartsWith("did ") ||
			//    text.StartsWith("there was ") ||
			//   text.Contains("were") ||
			//   text.Contains("are ") ||
			//    text.Contains("does ") ||
			//    text.Contains("do ") ||
			//	text.Contains("is it true that "))
			if(speech.TopIntent.Intent == Intents.GUESS)
			{
				//crack into the facts.
				SearchMind(speech);

			}
			else {
				//must be a yes/no question
				DependencyService.Get<ITextToSpeech>().Speak(VoiceLines.ASK_YES_NO);
			}
		}




		public void SearchMind(SpeechResult speech)
		{
			//this is where the magic happens...
			textToSay = "";

			//does it have negation? btw... GOTTA be a better way to do this... LUIS?
			var text = speech.Name.ToLowerInvariant();
			var hasNegation = text.Contains("wasn't ") ||
								text.Contains("isn't ") ||
								text.Contains("is not ") ||
			                      text.Contains("cannot ") ||
			                      text.Contains("can't ") ||
			                      text.Contains("will not ") ||
			                      text.Contains("won't ") ||
			                      text.Contains("didn't ") ||
			                      text.Contains("don't ") ||
			                      text.Contains("do not ") ||
			                      text.Contains("did not ") ||
								text.Contains("wasn't ") ||
								text.Contains("weren't ") ||
								text.Contains("not ") ||
								text.Contains("aren't ");

			//let's get any entities in the speech...
			var entities = speech.IntentResult.Entities;

			//search knowledge for key words. If there is a match, we have uncovered something.
			var facts = knowledge.Facts.Where(oo => entities.Select(ee => ee.Entity).ToList().Contains(oo.Subject)).ToList();
			var relevantFacts = new List<Fact>();
			var factAssociations = knowledge.Facts.Where(oo => entities.Select(ee => ee.Entity).ToList().Contains(oo.Association)).ToList();
			bool foundMatch = false;

			if(hasNegation)
			{
				facts = facts.Where(oo => !oo.PositiveAssociation).ToList();
				relevantFacts = facts.Where(oo => factAssociations.Contains(oo)).ToList();
				foundMatch = facts.Any(oo => factAssociations.Contains(oo));
			} else {
				facts = facts.Where(oo => oo.PositiveAssociation).ToList();
				relevantFacts = facts.Where(oo => factAssociations.Contains(oo)).ToList();
				foundMatch = facts.Any(oo => factAssociations.Contains(oo));
			}


			if (foundMatch)                           
			{
				//they are on to something!
				if(hasNegation)
				{
					textToSay += VoiceLines.CORRECT;
				} else {
					textToSay += VoiceLines.YES;
				}

				foreach (var fact in relevantFacts)
				{
					fact.WasLearned = true;
				}

				AssessProgress();
			} else {
				textToSay = VoiceLines.NO;
			}

			DependencyService.Get<ITextToSpeech>().Speak(textToSay);

		}


		public void AssessProgress()
		{
			var numFacts = knowledge.Facts.Count;
			var numLearnedFacts = knowledge.Facts.Where(oo => oo.WasLearned).Count();

			if (numLearnedFacts > 2 && numLearnedFacts <= 4) textToSay += VoiceLines.GETTING_CLOSER;
			if (numLearnedFacts > 4) textToSay += VoiceLines.FULL_STORY;
		}


		public void SetupMystery()
		{
			knowledge = new Knowledge();

			knowledge.Facts = new List<Fact>();

			var fact = new Fact
			{
				Subject = "thief",
				Association = "man"
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "man",
				Association = "robber"
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "man",
				Association = "burglar"
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "printing",
				Association = "it"
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "currency",
				Association = "plate"
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "plate",
				Association = "money"
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "plate",
				Association = "eat",
				PositiveAssociation = false
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "man",
				Association = "shot"
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "he",
				Association = "shot"
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "mint",
				Association = "candy",
				PositiveAssociation = false
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "mint",
				Association = "building"
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "mint",
				Association = "treasury"
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "mint",
				Association = "edible",
				PositiveAssociation = false
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "mint",
				Association = "eat",
				PositiveAssociation = false
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "mint",
				Association = "eaten",
				PositiveAssociation = false
			};
			knowledge.Facts.Add(fact);

			fact = new Fact
			{
				Subject = "the secret service",
				Association = "involved"
			};
			knowledge.Facts.Add(fact);



			knowledge.Clues = new List<Clue>();

			var clue = new Clue
			{
				Text = VoiceLines.CLUE1,
				Revealed = false
			};
			knowledge.Clues.Add(clue);

			clue = new Clue
			{
				Text = VoiceLines.CLUE2,
				Revealed = false
			};
			knowledge.Clues.Add(clue);

			DependencyService.Get<ITextToSpeech>().Speak(VoiceLines.WELCOME);


		}

	}


}

