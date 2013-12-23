using UnityEngine;
using System.Collections;

public class Character { //: MonoBehaviour {

	public CharacterStats stats;
	public string name;
	public bool isPC;
	public bool isDead;
	// maybe some references to graphic assets? like sprites or something?

	public Character()
	{
		name = "No Name";
		stats = new CharacterStats();
		isPC = false;
		isDead = false;
	}

	public Character( int str, int def, int mag, int agi)
	{
		name = "No Name";
		stats = new CharacterStats(str, def, mag, agi);
		isPC = false;
		isDead = false;
	}

	// not sure if this should return something
	public string PhysicalAttack( Character target)
	{
		Debug.Log(string.Format ("{0} is attacking {1}", name, target.name));
		// figure out if attack is successful
		if( stats.STR > target.stats.DEF )
		{
			// apply damage
			target.stats.HP -= 1; // TODO how much damage do we do?
			Debug.Log(string.Format("{0} does {1} damage to {2}",name, 1, target.name));
			return string.Format ("-1 ({0})", target.stats.HP);
		}
		return "0";
	}

	public string CastHeal( Character target)
	{
		int healAmt = stats.MAG;	// figure out how much healing is going to happen
		target.stats.HP += healAmt;	// apply to target
		if( target.stats.HP > target.stats.maxHP)
			target.stats.HP = target.stats.maxHP;
		Debug.Log(string.Format("{0} heals {1} HP to {2}",name, healAmt, target.name));
		return string.Format ("+1 ({0})", target.stats.HP);
	}


}
