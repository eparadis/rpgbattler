using UnityEngine;
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

		Character player;

		switch( bc.playerCharacter)
		{
		case 0:
			player = new Character(GameObject.Find("Hero"), 3, 1, 1, 2);	// GO str def mag agi
			player.name = "William (WIZ)";
			player.isPC = true;
			break;
		case 1:
			player = new Character(GameObject.Find("Hero"), 3, 2, 1, 1);
			player.name = "Nancy (KNI)";
			player.isPC = true;
			break;
		case 2:
			player = new Character(GameObject.Find("Hero"), 1, 3, 1, 2);
			player.name = "Clarence (CLR)";
			player.isPC = true;
			break;
		default:
			break;
		}

		switch( bc.level)
		{
		default:
			break;
		}

		PopulateTestCharacters(); // TODO implement the level and character loading!
	}
}
