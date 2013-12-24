﻿using UnityEngine;
using System.Collections;

public class Character { //: MonoBehaviour {

	public CharacterStats stats;
	public string name;
	public bool isPC;
	public bool isDead;
	// maybe some references to graphic assets? like sprites or something?
	public GameObject gfx;

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

	public string CastAttack( Character target)
	{
		int dmg = Random.Range( stats.MAG, stats.MAG*2);
		target.stats.HP -= dmg;
		Debug.Log(string.Format ("{0} does {1} damage to {2}", name, dmg, target.name));
		return string.Format ( "{0} ({1})", -dmg, target.stats.HP);
	}

	public string Defend()
	{
		// TODO make some sort of defend flag that is honored in attack and magic attack etc
		return "DEF up!";
	}

	// shake the character's icon/sprite back and forth some
	public IEnumerator ShakeAnimation( float length )
	{
		float startTime = Time.time;
		float amplitude = 0.1f;
		while( Time.time < startTime + length)
		{
			gfx.transform.position = new Vector3(Mathf.PingPong(Time.time, amplitude)-amplitude/2f + gfx.transform.position.x, gfx.transform.position.y, gfx.transform.position.z);
			yield return new WaitForSeconds(0.01f);	//TODO scale timing so that we always end on a complete cycle
		}
		yield return null;
	}

	public IEnumerator DeathAnimation()
	{
		// TODO the overall length, rotation amount per frame, and delay per frame calcs here are ALL WRONG >:O
		float speed = 5f;
		do {
			Debug.Log( gfx.transform.localEulerAngles.z );
			gfx.transform.Rotate(0, 0, speed);
			yield return new WaitForSeconds( (speed / 90f) * 0.5f );	// 0.5 seconds long 
		} while( gfx.transform.localEulerAngles.z <= 90f);
		yield return null;
	}

	// slide the character up to its target
	public IEnumerator ApproachTargetAnimation( Character target)
	{
		//TODO we'll need to figure out if the target is facing left or right, so we know what kind of offset to slide to
		// (negative for NPCs and positive for PCs)
		yield return null;
	}

	// slide the character back to it's 'home' position
	public IEnumerator ReturnHomeAnimation()
	{
		//TODO 'home' needs to be stored for each character, probably on startup/creation, so we can know where to go
		yield return null;
	}
}
