﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CharacterManager : MonoBehaviour {

	List<Character> allChars;

	public List<Character> GetAllChars()
	{
		return allChars;
	}

	public List<Character> GetLivingNPCs()
	{
		return allChars.FindAll ( delegate( Character z)
		                         {	return !z.isPC && !z.isDead;	} );
	}

	public List<Character> GetLivingPCs()
	{
		return allChars.FindAll ( delegate( Character z)
		                         {	return z.isPC && !z.isDead;	} );
	}

	// remove all chars that are marked as dead
	public void RemoveDead()
	{
		allChars.RemoveAll( delegate( Character z)
		                   { return z.isDead; } );
		// TODO  remove gfx of dead characters too!
	}

	// this should go away at some point... probably to some sort of Sprite or GFX manager
	public void ResetCharGfx()
	{
		foreach( Character ch in allChars)
		{
			ch.gfx.transform.eulerAngles = Vector3.zero;
		}
	}

	// Use this for initialization
	void Start () {
	}

	public void PopulateTestCharacters()
	{
		Character player = new Character(GameObject.Find("Hero"), 2, 1, 1, 3);
		player.name = "Fightin' Sam";
		player.isPC = true;

		Character enemyA = new Character(GameObject.Find("ghost"), 1, 1, 1, 1);
		enemyA.name = "Spooky Ghost";
		enemyA.isPC = false;
		enemyA.stats.maxHP = 2;

		Character enemyB = new Character(GameObject.Find("frog"), 2, 1, 1, 1);
		enemyB.name = "Turtle King";
		enemyB.isPC = false;
		enemyB.stats.maxHP = 2;

		allChars = new List<Character>();
		allChars.Add ( player);
		allChars.Add ( enemyA);
		allChars.Add ( enemyB);
	}

	public void PopulateCharacters()
	{
		BattleConfig bc = BattleConfig.GetSingleton();

		allChars = new List<Character>();
		Character player;

		switch( bc.playerCharacter)
		{
		case 0:
		default:
			GameObject wiz = (GameObject) GameObject.Instantiate( GameObject.Find("player ghost") );
			wiz.transform.position = new Vector3( -4.5f, -1.6f, 0);
			player = new Character( wiz , 3, 1, 1, 2);	// GO str def mag agi
			player.name = "Wilma (WIZ)";
			player.isPC = true;
			break;
		case 1:
			GameObject kni = (GameObject) GameObject.Instantiate( GameObject.Find("player hero") );
			kni.transform.position = new Vector3( -4.5f, -1.6f, 0);
			player = new Character( kni, 3, 2, 1, 1);
			player.name = "Nick (KNI)";
			player.isPC = true;
			break;
		case 2:
			GameObject clr = (GameObject) GameObject.Instantiate( GameObject.Find("player frog") );
			clr.transform.position = new Vector3( -4.5f, -1.6f, 0);
			player = new Character( clr, 1, 3, 1, 2);
			player.name = "Chris (CLR)";
			player.isPC = true;
			break;
		}
		allChars.Add(player);

		// level simply relates to how many enemies you are facing at this point
		for( int i=0; i<bc.level; i+=1)
		{
			GameObject ghost = (GameObject) GameObject.Instantiate(  GameObject.Find("ghost"));
			ghost.transform.position = new Vector3( 3.4f + (0.8f * (float)(i%2)), (4.8f - -2.5f) / (float) (bc.level+1) * i - 2.5f, 0);  // position the enemy along the right edge
			Character enemy = new Character( ghost, 2, 2, 1, 2); // make a character with the copy
			enemy.name = "Ghost " + (i+1);
			enemy.isPC = false;
			allChars.Add(enemy);
		}
	}
}
