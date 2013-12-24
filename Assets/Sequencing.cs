using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sequencing : MonoBehaviour {

	bool gameEnded;
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
		List<Character> charList = cm.GetAllChars();
		charList.Sort( delegate( Character x, Character y)
		              {  return y.stats.AGI.CompareTo( x.stats.AGI); } );		// reverse sort by AGI (DnD style)
		foreach( Character ch in charList)
		{
			if(ch.isDead)	// skip characters that were killed this round
				continue;

			if( ch.isPC == false)	// if AI controlled
			{
				// ai_mgr.DoBehavior(c, charList); // or i suppose it could do its own CharacterManager.GetChars()

				string result;
				if( Random.Range(0,2) == 0)
				{
					Character target = charList.Find ( delegate( Character z)
					                                  {	return z.isPC;	} );
					result = ch.PhysicalAttack( target );	// do a physical attack on the first PC in the list
					yield return StartCoroutine(ShowEnemyActionLabel( ch, "Attack " + result));
					yield return StartCoroutine(ch.ShakeAnimation(1f)); // show a graphic or animation
					yield return StartCoroutine(CheckForDeath(target));
				} else {
					result = ch.CastHeal( charList.Find ( delegate( Character z)
					                                    {	return !z.isPC;	} ) );	// cast 'heal' on the first non-PC in the list
					yield return StartCoroutine(ShowEnemyActionLabel( ch, "Heal " + result));
					yield return StartCoroutine(ch.ShakeAnimation(1f)); // show a graphic or animation
				}
			} else {
				yield return StartCoroutine(ShowPlayerBattleMenu( ch));
				// hide the menu and show a graphic or animation
			}
		}
		cm.RemoveDead();	// This is technically INTRODUCES a game design question. A character could die and be 
		// raised INSIDE a single round. He COULD lose his turn that round depending on ordering.
		// DnD style is to kill more or less immediately
		// All FF leaves the character "down", simply skipping his or her turn until 'raised'
		// The question then is whether Heal spells allow for raising or not.
		// Note: we're already skipping the turns of dead characters; the question here is whether to remove them from the battle field or not,
		// and if so, when to remove them.
		// As things work now, we have to remove them so they don't show up on menus and the game can tell everyone is dead.
		yield return null;
	}

	IEnumerator OuterLoop()
	{
		while(true)  // keep the game running forever ( i guess we use Application.Exit() to quit sometime in the future)
		{
			yield return StartCoroutine(ShowTitleUntilExit());	// show a title screen until the title screen is exited
			gameEnded = false;
			// show the choose a character screen until one is selected
			while(!gameEnded)// start a game and run it until the character dies (or they quit or something..)
			{
				yield return StartCoroutine(DoRound ());//   Do a round
				//   Check if that was the last round
				if( cm.GetNPCs().Count == 0)	// all the enemies are dead
				{
					Debug.Log( "Player has won by defeating all enemies");
					gameEnded = true;
				} else if( cm.GetPCs().Count == 0) // all your characters are dead
				{
					Debug.Log( "Player has lost by entire team dying");
					gameEnded = true;
				}
			}
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
		Character targetCharacter;
		string result;
		int actionSelection = 0;
		int menuSize = 4;
		string[] menu = { "Attack", "Defend", "M.Attack", "M.Heal" };
		// loop grabbing UP and DOWN keys until RETURN is pressed
		while( !Input.GetKeyDown(KeyCode.Return) ) 
		{
			// draw the menu with a selector arrow on the current option
			string menuText = "--" + ch.name + "--";
			for(int i=0; i<menuSize; i+=1)
			{
				if(actionSelection == i)
					menuText += "\n> ";
				else 
					menuText += "\n   ";
				menuText += menu[i];
			}
			guiText.text = menuText;

			// move arrow if UP/DOWN key pressed
			if( Input.GetKeyDown(KeyCode.UpArrow) )
			{
				actionSelection -= 1;
				if( actionSelection < 0)
					actionSelection = menuSize - 1;
			}
			if( Input.GetKeyDown(KeyCode.DownArrow) )
				actionSelection = (actionSelection + 1) % menuSize;
			yield return null;
		}

		// wait for a single frame so that we get another Input event (so we don't immediately select in the next section)
		yield return new WaitForEndOfFrame();

		// some actions require selecting an enemy or friendly target
		if(actionSelection == 0 ||	// attack
		   actionSelection == 2 )	// magic attack
		{
			// make a list of the enemies
			List<Character> enemiesChars = cm.GetNPCs();
			enemiesChars.Sort( delegate(Character x, Character y) {
								return x.name.CompareTo(y.name); });	// put in alphabetical order by name
			string[] names = new string[enemiesChars.Count];
			for(int i=0; i<enemiesChars.Count; i+=1)
				names[i] = enemiesChars[i].name;
			yield return StartCoroutine( GenericSelectionMenu( "Choose target", names));
			targetCharacter = enemiesChars[genericMenuSelection];
			// now do the thing
			if( actionSelection == 0)
			{
				result = ch.PhysicalAttack( targetCharacter);
				yield return StartCoroutine(ShowPlayerActionLabel( ch, "Attack " + result));
				yield return StartCoroutine(ch.ShakeAnimation(1f)); // show a graphic or animation
				yield return StartCoroutine(CheckForDeath(targetCharacter));
			} else {
				//ch.MagicAttack( targetCharacter);	// though i guess you'll have to select a spell to attack with
				result = "...thbbbt";
				yield return StartCoroutine(ShowPlayerActionLabel( ch, "Magic attack " + result));
				yield return StartCoroutine(ch.ShakeAnimation(1f)); // show a graphic or animation
				yield return StartCoroutine(CheckForDeath(targetCharacter));
			}
		} else if( actionSelection == 3)	// heal
		{
			// make a list of friendlies to heal (including yourself)
			List<Character> playerChars = cm.GetPCs();
			playerChars.Sort( delegate(Character x, Character y) {
								return x.name.CompareTo(y.name); });	// put in alphabetical order by name
			string[] names = new string[playerChars.Count];
			for(int i=0; i<playerChars.Count; i+=1)
				names[i] = playerChars[i].name;
			yield return StartCoroutine( GenericSelectionMenu( "Choose target", names));
			targetCharacter = playerChars[genericMenuSelection];

			result = ch.CastHeal( targetCharacter);
			yield return StartCoroutine(ShowPlayerActionLabel( ch, "Heal " + result));
			yield return StartCoroutine(ch.ShakeAnimation(1f)); // show a graphic or animation
		} else if( actionSelection == 1)	// defend
		{
			//pc.SetDefending(true); // or something like hat
			result = "...thbbbt";
			yield return StartCoroutine(ShowPlayerActionLabel( ch, "Defend " + result));
			yield return StartCoroutine(ch.ShakeAnimation(1f)); // show a graphic or animation
		}

		yield return null;
	}

	int genericMenuSelection;
	IEnumerator GenericSelectionMenu( string title, string[] options)
	{
		genericMenuSelection = 0;
		int menuSize = options.Length;
		while( !Input.GetKeyDown(KeyCode.Return) ) 
		{
			// draw the menu with a selector arrow on the current option
			string menuText = title.Clone () as string;
			for(int i=0; i<menuSize; i+=1)
			{
				if(genericMenuSelection == i)
					menuText += "\n> ";
				else 
					menuText += "\n   ";
				menuText += options[i];
			}
			guiText.text = menuText;
			
			// move arrow if UP/DOWN key pressed
			if( Input.GetKeyDown(KeyCode.UpArrow) )
			{
				genericMenuSelection -= 1;
				if( genericMenuSelection < 0)
					genericMenuSelection = menuSize - 1;
			}
			if( Input.GetKeyDown(KeyCode.DownArrow) )
				genericMenuSelection = (genericMenuSelection + 1) % menuSize;
			yield return null;
		}
		Debug.Log("Generic menu selection = " + genericMenuSelection);
	}

	IEnumerator ShowEnemyActionLabel( Character ch, string action)
	{
		string text = "--" + ch.name + "--\n  " + action;
		guiText.text = text;
		yield return null;
	}

	IEnumerator ShowPlayerActionLabel( Character ch, string action)
	{
		string text = "--" + ch.name + "--\n  " + action;
		guiText.text = text;
		yield return null;
	}

	IEnumerator CheckForDeath( Character ch)
	{
		if(ch.stats.HP <= 0)
		{
			guiText.text = ch.name + " has died!";
			ch.isDead = true;
			yield return StartCoroutine(ch.DeathAnimation()); // show a graphic or animation
		}
		else yield return StartCoroutine(ch.ShakeAnimation(0.2f)); // show a 'hurt' animation here
	}
}
