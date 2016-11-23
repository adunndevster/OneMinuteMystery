using System;
using System.Collections.Generic;
namespace OneMinuteMystery
{
	public class Knowledge
	{
		public Knowledge()
		{
			
		}

		public List<Fact> Facts { get; set; }
		public List<Clue> Clues { get; set; }
	}

	public class Fact
	{
		public Fact()
		{
			Id = 0;
			Subject = "";
			Association = "";
			WasLearned = false;
			PositiveAssociation = true;
		}

		public int Id { get; set; }
		public string Subject { get; set; }
		public string Association { get; set; }
		public bool PositiveAssociation { get; set;} //this just means the Subject "is" the Association in some way... eg. "The man _was_ shot"
		public bool WasLearned { get; set;} //tells us if the players have "uncovered" this facts
	}

	public class Clue
	{
		public int Id { get; set; }
		public string Path { get; set; } //path to the clue audio file
		public string Text { get; set; }
		public bool Revealed { get; set; }
	}
}
