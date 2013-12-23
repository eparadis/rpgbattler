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

	// Use this for initialization
	void Start () {
		PopulateTestCharacters();
	}

	void PopulateTestCharacters()
	{
		Character player = new Character(2, 1, 1, 3);
		player.name = "Fightin' Sam";
		player.isPC = true;

		Character enemy = new Character( 1, 1, 1, 1);
		enemy.name = "Bad Dude";
		enemy.isPC = false;
		enemy.stats.maxHP = 4;

		allChars = new List<Character>();
		allChars.Add ( player);
		allChars.Add ( enemy);
	}
}
