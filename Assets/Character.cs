using UnityEngine;
using System.Collections;

public class Character { //: MonoBehaviour {

	public CharacterStats stats;
	public string name;
	public bool isPC;
	public bool isDead;
	// maybe some references to graphic assets? like sprites or something?
	public GameObject gfx;
	Vector3 homePos;
	public bool isDefending;

	public Character( GameObject graphic)
	{
		name = "No Name";
		stats = new CharacterStats();
		isPC = false;
		isDead = false;
		isDefending = false;
		gfx = graphic;
		homePos = gfx.transform.position;
	}

	public Character( GameObject graphic, int str, int def, int mag, int agi)
	{
		name = "No Name";
		stats = new CharacterStats(str, def, mag, agi);
		isPC = false;
		isDead = false;
		isDefending = false;
		gfx = graphic;
		homePos = gfx.transform.position;
	}

	public Character( GameObject graphic, CharacterStats cs)
	{
		name = "No Name";
		stats = cs;
		isPC = false;
		isDead = false;
		isDefending = false;
		gfx = graphic;
		homePos = gfx.transform.position;
	}

	// not sure if this should return something
	public string PhysicalAttack( Character target)
	{
		Debug.Log(string.Format ("{0} is attacking {1}", name, target.name));
		if( target.isDefending)
		{
			if( stats.STR > target.stats.DEF*2)
			{
				target.stats.HP -= 1; // TODO need to rething how physical attacks work in regards to defending
				Debug.Log(string.Format("{0} does {1} damage to {2}",name, 1, target.name));
				return string.Format ("-1 ({0})", target.stats.HP);
			} else
				return "0";
		} else {
			// figure out if attack is successful
			if( stats.STR > target.stats.DEF )
			{
				// apply damage
				target.stats.HP -= 1; // TODO how much damage do we do?
				Debug.Log(string.Format("{0} does {1} damage to {2}",name, 1, target.name));
				return string.Format ("-1 ({0})", target.stats.HP);
			} else
				return "0";
		}
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
		int dmg = Random.Range( stats.MAG+stats.AGI, (stats.MAG+stats.AGI)*2);
		if(target.isDefending)
			dmg /= 2;	// TODO how does defending affect magical attacks?
		target.stats.HP -= dmg;
		Debug.Log(string.Format ("{0} does {1} damage to {2}", name, dmg, target.name));
		return string.Format ( "{0} ({1})", -dmg, target.stats.HP);
	}

	public string Defend()
	{
		// the defending flag should be honored when calculating any sort of damage effects
		// though I guess spells could avoid it?
		isDefending = true;
		return "Defensive stance!";
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
		Animator an = gfx.GetComponent<Animator>();
		if(an != null)
		{
			an.SetBool("isDead", true);
		}
		yield return null;
	}

	// slide the character up to its target
	public IEnumerator ApproachTargetAnimation( Character target)
	{
		// we need to figure out if the target is facing left or right, so we know what kind of offset to slide to
		// (negative for NPCs and positive for PCs)
		Vector3 stop;
		if( target.isPC)
			stop = target.gfx.transform.position + new Vector3(  1, 0, 0);
		else
			stop = target.gfx.transform.position + new Vector3( -1, 0, 0);

		float startTime = Time.time;
		float speed = 8f;
		float journeyLength = Vector3.Distance( homePos, stop);
		while(gfx.transform.position != stop)
		{
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			gfx.transform.position = Vector3.Lerp(homePos, stop, fracJourney);
			yield return null;
		}

		yield return null;
	}

	// slide the character back to it's 'home' position
	public IEnumerator ReturnHomeAnimation()
	{
		// 'home' needs to be stored for each character, probably on startup/creation, so we can know where to go
		Vector3 start = gfx.transform.position;

		float startTime = Time.time;
		float speed = 8f;
		float journeyLength = Vector3.Distance( start, homePos);
		while(gfx.transform.position != homePos)
		{
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			gfx.transform.position = Vector3.Lerp(start, homePos, fracJourney);
			yield return null;
		}
		
		yield return null;
	}

	public IEnumerator ShootSparklies(Color c)
	{
		// we need some particles to be emitted from the character 
		ParticleSystem ps = gfx.GetComponent<ParticleSystem>();
		if( ps != null)
		{
			ps.startColor = c;
			ps.startSpeed = 2; // outwards
			ps.Play();
			yield return new WaitForSeconds( 1f);
			ps.Stop();
			ps.Clear();
		}

		yield return null;
	}

	public IEnumerator AttractSparklies(Color c)
	{
		ParticleSystem ps = gfx.GetComponent<ParticleSystem>();
		if( ps != null)
		{
			ps.startColor = c;
			ps.startSpeed = -2; // inwards
			ps.Play();
			yield return new WaitForSeconds( 1f);
			ps.Stop();
			ps.Clear();
		}
		
		yield return null;
	}

	public IEnumerator CastAnimation()
	{
		Animator an = gfx.GetComponent<Animator>();
		if(an != null)
		{
			an.SetInteger("Action", 2);
		}
		yield return null;
	}

	public IEnumerator IdleAnimation()
	{
		Animator an = gfx.GetComponent<Animator>();
		if(an != null)
		{
			an.SetInteger("Action", 0);
		}
		yield return null;
	}

	public IEnumerator DefendAnimation()
	{
		Animator an = gfx.GetComponent<Animator>();
		if(an != null)
		{
			an.SetInteger("Action", 3);
		}
		yield return null;
	}

	public IEnumerator StabAnimation()
	{
		Animator an = gfx.GetComponent<Animator>();
		if(an != null)
		{
			//an.SetInteger("Action", 1);
			an.SetTrigger("doStab");
			yield return new WaitForSeconds(1f);
		}
		yield return null;
	}

	public IEnumerator StruckAnimation()
	{
		Animator an = gfx.GetComponent<Animator>();
		if(an != null)
		{
			//an.SetInteger("Action", 4);
			an.SetTrigger("doStruck");
		}
		yield return null;
	}
}
