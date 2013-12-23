using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sequencing : MonoBehaviour {

	bool gameEnded = false;
	CharacterManager cm;

	// Use this for initialization
	void Start () {
		cm = GetComponent<CharacterManager>();
		StartCoroutine( "OuterLoop");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// in a round, all characters/enemies take a turn
	//  I think in the future, instead of AGI determining order, it determines the rate of repeats (DnD style vs later FF style, basically)
	IEnumerator DoRound()
	{
		List<Character> charList = cm.GetChars();
		charList.Sort( delegate( Character x, Character y)
		              {  return y.stats.AGI.CompareTo( x.stats.AGI); } );		// reverse sort by AGI (DnD style)
		foreach( Character c in charList)
		{
			if( c.isPC == false)	// if AI controlled
			{
				// ai_mgr.DoBehavior(c, charList); // or i suppose it could do its own CharacterManager.GetChars()

				string result;
				if( Random.Range(0,2) == 0)
				{
					result = c.PhysicalAttack( charList.Find ( delegate( Character z)
					                                          {	return z.isPC;	} ) );	// do a physical attack on the first PC in the list
					yield return StartCoroutine(ShowEnemyActionLabel( c, "Attack " + result));
				} else {
					result = c.CastHeal( charList.Find ( delegate( Character z)
					                                    {	return !z.isPC;	} ) );	// cast 'heal' on the first non-PC in the list
					yield return StartCoroutine(ShowEnemyActionLabel( c, "Heal " + result));
				}
				// show a graphic or animation
				yield return new WaitForSeconds( 1f );
			} else {
				yield return StartCoroutine(ShowPlayerBattleMenu( c));
				// hide the menu and show a graphic or animation
			}

		}
		yield return null;
	}

	IEnumerator OuterLoop()
	{
		while(true)  // keep the game running forever ( i guess we use Application.Exit() to quit sometime in the future)
		{
			yield return StartCoroutine(ShowTitleUntilExit());	// show a title screen until the title screen is exited
			// show the choose a character screen until one is selected
			while(!gameEnded)// start a game and run it until the character dies (or they quit or something..)
				yield return StartCoroutine(DoRound ());//   Do a round
				//   Check if that was the last round

		}
	}

	IEnumerator ShowTitleUntilExit()
	{
		guiText.text = "RPG Battler\nby Ed P\nPress space to start";
		while( !Input.GetKeyDown(KeyCode.Space) )
			yield return null;
	}

	// present the user with a menu of actions in battle
	IEnumerator ShowPlayerBattleMenu( Character ch)
	{
		int selection = 0;
		int menuSize = 4;
		string[] menu = { "Attack", "Defend", "M.Attack", "M.Heal" };
		// loop grabbing UP and DOWN keys until RETURN is pressed
		while( !Input.GetKeyDown(KeyCode.Return) ) 
		{
			// draw the menu with a selector arrow on the current option
			string menuText = "--" + ch.name + "--";
			for(int i=0; i<menuSize; i+=1)
			{
				if(selection == i)
					menuText += "\n> ";
				else 
					menuText += "\n   ";
				menuText += menu[i];
			}
			guiText.text = menuText;

			// move arrow if UP/DOWN key pressed
			if( Input.GetKeyDown(KeyCode.UpArrow) )
				selection = (selection - 1) % menuSize;
			if( Input.GetKeyDown(KeyCode.DownArrow) )
				selection = (selection + 1) % menuSize;
			yield return null;
		}
		yield return null;
	}

	IEnumerator ShowEnemyActionLabel( Character ch, string action)
	{
		string text = "--" + ch.name + "--\n  " + action;
		guiText.text = text;
		yield return null;
	}
}
