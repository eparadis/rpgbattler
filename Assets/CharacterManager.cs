using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CharacterManager : MonoBehaviour {

	List<Character> chars;

	public List<Character> GetChars()
	{
		return chars;
	}

	// Use this for initialization
	void Start () {
		PopulateTestCharacters();
	}

	void PopulateTestCharacters()
	{
		Character player = new Character(2, 1, 1, 3);
		player.name = "Player";
		player.isPC = true;

		Character enemy = new Character( 1, 1, 1, 1);
		enemy.name = "Enemy";

		chars = new List<Character>();
		chars.Add ( player);
		chars.Add ( enemy);
	}
}
