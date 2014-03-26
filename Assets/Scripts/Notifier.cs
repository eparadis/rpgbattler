using UnityEngine;
using System.Collections;


// handles status and effect notifications (such as the damage numbers that float above a character's head, or low health warnings, etc)
//  things that have their own time line, but don't stop the game or need user input

public class Notifier { // : MonoBehaviour {

	Character self;
	GameObject textGO;
	TextMesh textMesh;

	public Notifier( Character ch )
	{
		self = ch;

		textGO = new GameObject("notifier");
		textGO.transform.parent = self.gfx.transform;
		textGO.transform.localPosition = new Vector3( 0f, 1.5f, -1f);

		if( self.gfx.GetComponent<SpriteCharacterBuilder>().facingLeft )	// if the graphic is flipped
			textGO.transform.localScale = new Vector3( -1, 1, 1);	// flip the text, so that its correct (ie: double flipped)

		textMesh = textGO.AddComponent<TextMesh>();
		//Font font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		Font font = GameObject.Find("logic").GetComponent<GUIText>().font;
		textMesh.font = font;
		textMesh.renderer.material = font.material;
		textMesh.fontSize = 16;
		textMesh.characterSize = 0.3f;
		textMesh.alignment = TextAlignment.Center;
		textMesh.anchor = TextAnchor.MiddleCenter;
		textMesh.text = "";
	}


	public IEnumerator ShowActionLabel( string action) //, Color color = Color.white )
	{
		//Debug.Log ("inside show action label");
		//string text = "--" + self.name + "--\n  " + action;
		string text = action;
		textMesh.text = text;
		//textMesh.color = color;

		// launch this but don't yield on it, so the game doesn't 'block' on it
		self.helper.StartCoroutine(FloatTextUpwardsAndDisappear());
		yield return null;
	}

	IEnumerator FloatTextUpwardsAndDisappear()
	{
		float startTime = Time.time;
		float speed = 1f;
		while( Time.time < startTime + 0.75f)
		{
			// TODO animate the text floating up or something
			textGO.transform.localPosition = Vector3.MoveTowards( textGO.transform.localPosition, new Vector3(0f, 3f, -1f), Time.deltaTime * speed);
			// TODO probably need to launch a coroutine to go do the animation and clear the text later
			yield return null;
		}
		textMesh.text = "";	// clear when we're done animating
		textGO.transform.localPosition = new Vector3( 0f, 1.5f, -1f);
	}


}
