using UnityEngine;
using System.Collections;

public class Character { //: MonoBehaviour {

	public CharacterStats stats;
	public string name;
	public bool isPC;
	// maybe some references to graphic assets? like sprites or something?

	public Character()
	{
		name = "No Name";
		stats = new CharacterStats();
		isPC = false;
	}

	public Character( int str, int def, int mag, int agi)
	{
		name = "No Name";
		stats = new CharacterStats(str, def, mag, agi);
		isPC = false;
	}
}
