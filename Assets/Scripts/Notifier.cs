using UnityEngine;
using System.Collections;


// handles status and effect notifications (such as the damage numbers that float above a character's head, or low health warnings, etc)
//  things that have their own time line, but don't stop the game or need user input

public class Notifier { // : MonoBehaviour {

	GUIText guiText;
	Character self;

	public Notifier( Character ch )
	{
		self = ch;
		guiText = self.gfx.GetComponent<GUIText>();
		if( guiText == null)
		{
			guiText = self.gfx.AddComponent<GUIText>();
			Debug.Log("notifier created on " + ch.name );
		}

		guiText.color = Color.red; // set fonts and colors or whatever
		// TODO set position to the character's graphic somehow
	}


	public IEnumerator ShowActionLabel( string action)
	{
		Debug.Log ("inside show action label");
		string text = "--" + self.name + "--\n  " + action;
		guiText.text = text;
		// TODO animate the text floating up or something
		// TODO probably need to launch a coroutine to go do the animation and clear the text later
		// TODO also make sure it stays above the our Character (self)
		yield return null;
	}
}
