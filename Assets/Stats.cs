using UnityEngine;
using System.Collections;


class CharacterStats {
	public int HP;
	public int XP;
	public int LVL;

	public int STR;
	public int DEF;
	//int MAG;
	int AGI;

	void CalcHP()
	{
		HP = DEF + STR + 5;
	}

	// adds XP to this characters stats, updates the char's level, and returns true if the char leveled
	bool AddXP( int add)
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


}
