using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CharacterManager : MonoBehaviour {

	List<Character> allChars;

	public List<Character> GetAllChars()
	{
		return allChars;
	}

	public List<Character> GetNPCs()
	{
		return allChars.FindAll ( delegate( Character z)
		                         {	return !z.isPC;	} );
	}

	public List<Character> GetPCs()
	{
		return allChars.FindAll ( delegate( Character z)
		                         {	return z.isPC;	} );
	}

	// remove all chars that are marked as dead
	public void RemoveDead()
	{
		allChars.RemoveAll( delegate( Character z)
		                   { return z.isDead; } );
		// TODO  remove gfx of dead characters too!
	}

	// Use this for initialization
	void Start () {
		PopulateTestCharacters();
	}

	void PopulateTestCharacters()
	{
		Character player = new Character(2, 1, 1, 3);
		player.name = "Fightin' Sam";
		player.isPC = true;
		player.gfx = GameObject.Find("blue quad");

		Character enemyA = new Character( 1, 1, 1, 1);
		enemyA.name = "Bad Dude A";
		enemyA.isPC = false;
		enemyA.stats.maxHP = 2;
		enemyA.gfx = GameObject.Find("red cube A");

		Character enemyB = new Character( 1, 1, 1, 1);
		enemyB.name = "Bad Dude B";
		enemyB.isPC = false;
		enemyB.stats.maxHP = 2;
		enemyB.gfx = GameObject.Find("red cube B");

		allChars = new List<Character>();
		allChars.Add ( player);
		allChars.Add ( enemyA);
		allChars.Add ( enemyB);
	}
}
