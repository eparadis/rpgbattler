using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CharacterManager : MonoBehaviour {

	public AudioClip genericAttackSfx, genericDefendSfx, genericMagAttackSfx, genericHealSfx, genericBattleEnterSfx, genericDeathSfx;

	List<Character> allChars;

	public List<Character> GetAllChars()
	{
		return allChars;
	}

	public void SortCharactersByAGI()
	{
		// check if allChars is already sorted. we don't want to re-sort it and change the established order (ie: only sort when we really need to)

		// if its not sorted, do our special sort that only reorders the elements that aren't in order (again to perserve over all order as much as logicall possible)

		// TODO: don't simply use the default sorting method
		allChars.Sort( delegate( Character x, Character y)
		      {  return y.stats.AGI.CompareTo( x.stats.AGI); } );		// reverse sort by AGI (DnD style)
	}

	private void OrderPreservingSortByAGI()
	{
		// step through the list until something is out of order

		//   remove it

		//   put it back...?
	}

	private bool CharListIsSorted()
	{
		return true;
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

	private void SetGenericSfx( Character c)
	{
		c.attackSfx = genericAttackSfx;
		c.battleEnterSfx = genericBattleEnterSfx;
		c.deathSfx = genericDeathSfx;
		c.defendSfx = genericDefendSfx;
		c.healSfx = genericHealSfx;
		c.magAttackSfx = genericMagAttackSfx;
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
			player = new Character( wiz , bc.PCStats);
			player.name = "Wilma (WIZ)";
			player.isPC = true;
			SetGenericSfx(player);
			break;
		case 1:
			GameObject kni = (GameObject) GameObject.Instantiate( GameObject.Find("player hero") );
			kni.transform.position = new Vector3( -4.5f, -1.6f, 0);
			player = new Character( kni, bc.PCStats);
			player.name = "Nick (KNI)";
			player.isPC = true;
			SetGenericSfx(player);
			break;
		case 2:
			GameObject clr = (GameObject) GameObject.Instantiate( GameObject.Find("player frog") );
			clr.transform.position = new Vector3( -4.5f, -1.6f, 0);
			player = new Character( clr, bc.PCStats);
			player.name = "Chris\t (CLR)";
			player.isPC = true;
			SetGenericSfx(player);
			break;
		}
		allChars.Add(player);

		// level simply relates to how many enemies you are facing at this point
		for( int i=0; i<bc.level; i+=1)
		{
			GameObject ghost = (GameObject) GameObject.Instantiate(  GameObject.Find("ghost"));
			ghost.transform.position = new Vector3( 3.0f + (2.3f * (float)(i%2)), (4.8f - -2.5f) / (float) (bc.level+1) * i - 2.5f, 0);  // position the enemy along the right edge
			Character enemy = new Character( ghost, bc.level, bc.level, bc.level, bc.level); // make a character with the copy
			enemy.name = "Ghost " + (i+1);
			enemy.isPC = false;
			SetGenericSfx(enemy);
			allChars.Add(enemy);
		}

		// move all the characters off screen so they can animate in; we do this now so we can use the 'return home' animation already written
		foreach( Character ch in allChars)
		{
			if(ch.isPC)
				ch.gfx.transform.Translate( new Vector3( -3f, 0, 0));
			else
				ch.gfx.transform.position = new Vector3( 8f, 0, 0);
		}
	}
}
