using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace OneMinuteMystery
{
	[JsonObject("result")]
	public class SpeechResult
	{
		public string Scenario { get; set; }
		public string Name { get; set; }
		public string Lexical { get; set; }
		public string Confidence { get; set; }
		public IntentResult IntentResult { get; set;}


		public IntentObject TopIntent
		{
			get
			{
				var intent = new IntentObject();
				intent = IntentResult.Intents.OrderByDescending(oo => oo.Score).FirstOrDefault();
				return intent;
			}

			set { }

		}	
	}

	public class RootObject
	{
		public List<SpeechResult> results { get; set; }
	}

	public class IntentResult
	{
		public string Query { get; set; }
		public List<IntentObject> Intents {get; set;}
		public List<EntityObject> Entities { get; set;}
	}

	public class IntentObject
	{
		public string Intent { get; set; }
		public decimal Score { get; set; }
	}
	public class EntityObject
	{
		public string Entity { get; set; }
		public string Type { get; set;}
		public int StartIndex { get; set;}
		public int EndIndex { get; set; }
		public decimal Score { get; set; }
	}
}
