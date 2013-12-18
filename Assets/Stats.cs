using UnityEngine;
using System.Collections;


class CharacterStats {
	public int HP;
	public int XP;
	public int LVL;

	public int STR;
	public int DEF;
	//int MAG;
	public int AGI;

	void CalcHP()
	{
		HP = DEF + STR + 5;
	}

	// adds XP to this characters stats, updates the char's level, and returns true if the char leveled
	public bool AddXP( int add)
	{
		//           2   3    4    5     6     7 ...
		// level at 30, 90, 270, 810, 2430, 7290 ( n x 3)
		XP += add;
		if( XP >= Mathf.Pow(3, LVL)*10)
		{
			LVL += 1;
			return true;
		}
		return false;
	}

	public CharacterStats()
	{
		STR = DEF = 1;
		CalcHP();
		XP = 0;
		LVL = 1;
	}
};

public class Stats : MonoBehaviour {

	void Attack( int attackStr, CharacterStats defender)
	{
		if( attackStr > defender.DEF + defender.AGI)
		{
			// hit, do damage
			defender.HP -= attackStr - defender.DEF;
		}
	}

	void TestCase()
	{
		CharacterStats fighter = new CharacterStats();
		for(int i=0; i<10; i+=1)
		{
			int xp = (i+1)*10;
			Debug.Log("figher gained " + xp + " XP");
			if(fighter.AddXP(xp))
			{
				Debug.Log("fighter leveled! Now level " + fighter.LVL);
				Debug.Log("putting points into STR");
				fighter.STR += 4;
			}
		}

		CharacterStats enemy = new CharacterStats();
		Attack (enemy.STR, fighter);
	}

	void Start()
	{
		TestCase();
	}
}

/*
 * so it makes sense that with 4 main stats, you get 4 points each level.
 * thus at level 1, you coudl have a 1-1-1-1 char
 *      at level 2, you could have a 2-2-2-2 char
 *                             OR  a 5-1-1-1 char or any other of many combinations
 * so customization is available right from the start
 * 
 * another idea would be to give one less than the number of main stats (3 in this case)
 * then players would need to choose how to specialize from the begining
 * 
 * the basic options when battling should be:
 * - attack with physical weapon (based on STR and AGI)
 * - defend (bonus to DEF that depends on ? AGI ? )
 * - attack with magic ( based on MAG + ?)
 * - heal with magic ( based on MAG + ?)
 * 
 * I'm not sure how spells should happen.  Is there simply two magic-related things (m.attack and m.heal)
 * or should there be a Spell menu, with various spells (which include Heal and things like 'fireball')
 * 
 * Having spells complicates it, but gives the opportunity for area-effect attacks, or status attacks.
 * The trade-off for magic is that it should use some resource.  Perhaps that resource is simply your MAG
 * The power of your spells are fixed based on the spell level, and higher level spells are unlocked as the character levels
 * It'd have to be tied to both MAG and LVL somehow, else a lvl 15 fighter, with no points in MAG would see spells as options he couldn't cast
 * 
 * There probably needs to be some cap on the number of points that can go into a single stat
 * lvl	STR	DEF MAG AGI
 * 1	1	1	1	1
 * 2	5	1	1	1
 * 3	9	1	1	1
 * 4	13	1	1	1
 * etc
 * 
 * A char like this would probably kill anything in a single hit, but his AGI would be so low, he's go last in each round and die because
 * he has such low DEF
 * Depending on how HP is calculated
 * HP = STR + DEF + 5 would make sense (putting points in MAG makes you 'weaker') but could be abused
 * HP = (STR + DEF) / 2 + 5 results in a lower HP (Which could be tuned), but prevents really unbalanced characters
 * 
 * Starting conditions
 * the "level 1 = 1-1-1-1" is just an assumption.  A player could perhaps choose different starting characters to help tune their party
 * perhaps 2-1-0-1 or 2-2-0-0 (and MAG analogs)
 * 
 * 
 * 
 * */
