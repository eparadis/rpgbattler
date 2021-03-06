﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleSequencing : MonoBehaviour {

	bool levelEnded;
	CharacterManager cm;
	BattleConfig bc;
	SfxManager sfx;
	public AudioClip menuSelect;
	public AudioClip menuAccept;

	// Use this for initialization
	void Start () {
		cm = GetComponent<CharacterManager>();
		bc = BattleConfig.GetSingleton();
		sfx = SfxManager.GetSingleton();
		StartCoroutine( "OuterLoop");
	}

	// runs a single game; ie: a series of rounds until the player loses or wins
	IEnumerator OuterLoop()
	{
		levelEnded = false;
		cm.PopulateCharacters();	// load the PC and NPCs
		yield return StartCoroutine( AllCharactersEnterBattle()); // everyone slides in from off screen!

		while(!levelEnded)// start a game and run it until the character dies (or they quit or something..)
		{
			yield return StartCoroutine(DoRound ());//   Do a round
			//   Check if that was the last round
			if( cm.GetLivingNPCs().Count == 0)	// all the enemies are dead
			{
				guiText.text = "Hurrah!\nYou have defeated this\nparty of enemies!";
				Debug.Log( "Player has won by defeating all enemies");
				levelEnded = true;
				bc.level += 1;	// advance to next level!
				yield return new WaitForSeconds(3f);	// TODO replace with some sort of victory animation
				Application.LoadLevel ( "leveling" ); // after winning, you get to level your character!
			} else if( cm.GetLivingPCs().Count == 0) // all your characters are dead
			{
				guiText.text = "Too bad!\nYou have been\ndefeated.";
				Debug.Log( "Player has lost by entire team dying");
				levelEnded = true;
				yield return new WaitForSeconds(3f);	// TODO replace with some sort of loss animation
				// TODO show high score screen (your score would be the level you completed, I suppose)
				Application.LoadLevel (0); // zero is the title
			}
		}
		// this would be reached if the level ended without a win or a loss, such as saving or quitting
	}

	// in a round, all characters/enemies take a turn
	//  I think in the future, instead of AGI determining order, it determines the rate of repeats (DnD style vs later FF style, basically)
	IEnumerator DoRound()
	{
		List<Character> charList = cm.GetAllChars();
		charList.Sort( delegate( Character x, Character y)
		              {  return y.stats.AGI.CompareTo( x.stats.AGI); } );	// reverse sort by AGI (DnD style)
		foreach( Character ch in charList)
		{
			if(ch.isDead)	// skip characters that were killed this round
				continue;
			if( ch.isPC )
				yield return StartCoroutine(ShowPlayerBattleMenu( ch));
			else
				yield return StartCoroutine(ch.behavior.TakeTurn( charList) );	// ask the NPC character to do whatever its behavior is
		}
		//cm.RemoveDead();	// This is technically INTRODUCES a game design question. A character could die and be 
		// raised INSIDE a single round. He COULD lose his turn that round depending on ordering.
		// DnD style is to kill more or less immediately
		// All FF leaves the character "down", simply skipping his or her turn until 'raised'
		// The question then is whether Heal spells allow for raising or not.
		// Note: we're already skipping the turns of dead characters; the question here is whether to remove them from the battle field or not,
		// and if so, when to remove them.
		// As things work now, we have to remove them so they don't show up on menus and the game can tell everyone is dead.
		yield return null;
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
		string[] menu = { "Attack", "Defend", "M.Attack", "M.Heal" };
		yield return StartCoroutine( GenericSelectionMenu( "--" + ch.name + "--", menu));
		int actionSelection = genericMenuSelection;

		// wait for a single frame so that we get another Input event (so we don't immediately select in the next section)
		yield return new WaitForEndOfFrame();

		// some actions require selecting an enemy or friendly target
		if(actionSelection == 0 ||	// attack
		   actionSelection == 2 )	// magic attack
		{
			// make a list of the enemies
			List<Character> enemiesChars = cm.GetLivingNPCs();
			enemiesChars.Sort( delegate(Character x, Character y) {
								return x.name.CompareTo(y.name); });	// put in alphabetical order by name
			string[] names = new string[enemiesChars.Count];
			for(int i=0; i<enemiesChars.Count; i+=1)
				names[i] = enemiesChars[i].name;
			yield return StartCoroutine( GenericSelectionMenu( "Choose target:", names));
			targetCharacter = enemiesChars[genericMenuSelection];
			// now do the thing
			if( actionSelection == 0)
			{
				result = ch.PhysicalAttack( targetCharacter);
				yield return StartCoroutine(ch.notifier.ShowActionLabel( "Attack " + result));
				yield return StartCoroutine(ch.IdleAnimation());
				yield return StartCoroutine(ch.ApproachTargetAnimation( targetCharacter));
				sfx.Play(ch.attackSfx);
				yield return StartCoroutine(ch.StabAnimation());
				yield return StartCoroutine(targetCharacter.CheckForDeath() );
				yield return StartCoroutine(ch.IdleAnimation());
				yield return StartCoroutine(ch.ReturnHomeAnimation());

			} else {
				result = ch.CastAttack( targetCharacter);
				sfx.Play(ch.magAttackSfx);
				yield return StartCoroutine(ch.notifier.ShowActionLabel( "Magic attack " + result));
				yield return StartCoroutine(ch.CastAnimation() );
				yield return StartCoroutine(ch.ShootSparklies( Color.red ) ); 
				yield return StartCoroutine(ch.IdleAnimation() );
				yield return StartCoroutine( targetCharacter.StruckAnimation() );
				yield return StartCoroutine(targetCharacter.AttractSparklies( Color.red ) );
				yield return StartCoroutine( targetCharacter.IdleAnimation() );
				yield return StartCoroutine(targetCharacter.CheckForDeath());
			}
		} else if( actionSelection == 3)	// heal
		{
			// make a list of friendlies to heal (including yourself)
			List<Character> playerChars = cm.GetLivingPCs();
			playerChars.Sort( delegate(Character x, Character y) {
								return x.name.CompareTo(y.name); });	// put in alphabetical order by name
			string[] names = new string[playerChars.Count];
			for(int i=0; i<playerChars.Count; i+=1)
				names[i] = playerChars[i].name;
			yield return StartCoroutine( GenericSelectionMenu( "Choose target", names));
			targetCharacter = playerChars[genericMenuSelection];

			// do the stats effects and animations
			result = ch.CastHeal( targetCharacter);
			sfx.Play(ch.healSfx);
			yield return StartCoroutine(ch.notifier.ShowActionLabel( "Heal " + result));
			yield return StartCoroutine(ch.CastAnimation());
			yield return StartCoroutine(ch.ShootSparklies( Color.green ) ); 
			yield return StartCoroutine(targetCharacter.AttractSparklies( Color.green ) );
			yield return StartCoroutine(ch.IdleAnimation());
		} else if( actionSelection == 1)	// defend
		{
			result = ch.Defend();
			sfx.Play( ch.defendSfx);
			yield return StartCoroutine(ch.notifier.ShowActionLabel(result));
			yield return StartCoroutine(ch.DefendAnimation());
		}

		yield return null;
	}

	int genericMenuSelection;
	IEnumerator GenericSelectionMenu( string title, string[] options)
	{
		yield return new WaitForEndOfFrame();	// always wait for one frame to clear input buffer

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
				sfx.Play( menuSelect);
				genericMenuSelection -= 1;
				if( genericMenuSelection < 0)
					genericMenuSelection = menuSize - 1;
			}
			if( Input.GetKeyDown(KeyCode.DownArrow) )
			{
				sfx.Play( menuSelect);
				genericMenuSelection = (genericMenuSelection + 1) % menuSize;
			}
			yield return null;
		}
		sfx.Play( menuAccept);
		//Debug.Log("Generic menu selection = " + genericMenuSelection);
	}

	IEnumerator AllCharactersEnterBattle()
	{
		//Debug.Log("XXXXX all chars enter battle called");
		// we get a list of all the characters and sort them by who is going first, so that they animate into the battle field in order
		List<Character> charList = cm.GetAllChars();
		charList.Sort( delegate( Character x, Character y)
		              {  return y.stats.AGI.CompareTo( x.stats.AGI); } );		// reverse sort by AGI (DnD style)
		foreach( Character ch in charList)
		{
			sfx.Play (ch.battleEnterSfx);
			yield return StartCoroutine( ch.ReturnHomeAnimation() );
		}
	}
}
