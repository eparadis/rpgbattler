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
	public AudioClip attackSfx, defendSfx, magAttackSfx, healSfx, battleEnterSfx, deathSfx;
	public EnemyBehavior behavior;
	public Notifier notifier;
	public AudioSource sfx;
	public Helper helper;

	public Character( GameObject graphic)
	{
		name = "No Name";
		stats = new CharacterStats();
		isPC = false;
		isDead = false;
		isDefending = false;
		gfx = graphic;
		homePos = gfx.transform.position;
		sfx = GetSfx();
		helper = GetHelper();
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
		sfx = GetSfx();
		helper = GetHelper();
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
		sfx = GetSfx();
		helper = GetHelper();
	}

	private AudioSource GetSfx()
	{
		AudioSource ret;
		if(gfx != null)
		{
			ret = gfx.GetComponent<AudioSource>();
			if( ret == null)
				ret = gfx.AddComponent<AudioSource>();
			ret.loop = false;
			ret.playOnAwake = false;
			ret.volume = 0.25f;	//TODO look up sfx volume from some global place
			// TODO what other things do we need to init here?
			return ret;
		} else {
			Debug.LogWarning("Character could not get or create an AudioSource because its gfx was unset!");
			return null;
		}
	}

	private Helper GetHelper()
	{
		Helper ret;
		GameObject go = GameObject.Find("logic");
		if( go != null)
		{
			ret = go.GetComponent<Helper>();
			if( ret == null)
				ret = go.AddComponent<Helper>();
			return ret;
		} else {
			Debug.LogWarning("Character could not get or create a Helper because gameobject was unset!");
			return null;
		}
	}

	// not sure if this should return something
	public string PhysicalAttack( Character target)
	{
		Debug.Log(string.Format ("{0} is attacking {1}", name, target.name));
		if( target.isDefending)
		{
			if( stats.STR > target.stats.DEF*2)
			{
				int dmg = stats.STR - target.stats.DEF;
				target.stats.HP -= dmg;
				Debug.Log(string.Format("{0} does {1} damage to {2}",name, dmg, target.name));
				return string.Format ("{0} ({1})", dmg, target.stats.HP);
			} else
				return "0";
		} else {
			// figure out if attack is successful
			if( stats.STR > target.stats.DEF )
			{
				// apply damage
				int dmg = stats.STR - target.stats.DEF;
				target.stats.HP -= dmg;
				Debug.Log(string.Format("{0} does {1} damage to {2}",name, dmg, target.name));
				return string.Format ("{0} ({1})", -dmg, target.stats.HP);
			} else
				return "0";
		}
	}

	public string CastHeal( Character target)
	{
		int healAmt = (stats.DEF + stats.MAG) / 2;
		target.stats.HP += healAmt;	// apply to target
		if( target.stats.HP > target.stats.maxHP)
			target.stats.HP = target.stats.maxHP;
		Debug.Log(string.Format("{0} heals {1} HP to {2}",name, healAmt, target.name));
		return string.Format ("+{0} ({1})", healAmt, target.stats.HP);
	}

	public string CastAttack( Character target)
	{
		int dmg = Random.Range( (stats.MAG+stats.AGI) / 4, (stats.MAG+stats.AGI)/2);
		if(target.isDefending)
			dmg /= 2;
		if( dmg == 0) dmg = 1;	// at least a point of damage
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
		SpriteAnimator sa = gfx.GetComponent<SpriteAnimator>();
		if(sa != null)
		{
			sa.Play("die");
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
		ParticleSystem ps = gfx.GetComponentInChildren<ParticleSystem>();
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
		ParticleSystem ps = gfx.GetComponentInChildren<ParticleSystem>();
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
		SpriteAnimator sa = gfx.GetComponent<SpriteAnimator>();
		if(sa != null)
		{
			sa.Play("cast");
		}
		yield return null;
	}

	public IEnumerator IdleAnimation()
	{
		SpriteAnimator sa = gfx.GetComponent<SpriteAnimator>();
		if(sa != null)
		{
			sa.Play("idle");
		}
		yield return null;
	}

	public IEnumerator DefendAnimation()
	{
		SpriteAnimator sa = gfx.GetComponent<SpriteAnimator>();
		if(sa != null)
		{
			sa.Play("defend");
		}
		yield return null;
	}

	public IEnumerator StabAnimation()
	{
		SpriteAnimator sa = gfx.GetComponent<SpriteAnimator>();
		if(sa != null)
		{
			sa.Play("stab");
		}
		yield return null;
	}

	public IEnumerator StruckAnimation()
	{
		SpriteAnimator sa = gfx.GetComponent<SpriteAnimator>();
		if(sa != null)
		{
			sa.Play("struck");
		}
		yield return null;
	}

	// this is a little wierd
	// on one hand, this could be part of Character, as its really only messing with Character stuff - target.CheckForDeath()
	// on the other hand, i could see how a character's response (maybe casting a last minute save, or running away) would be a 'behavior'
	//   and this should then go into EnemyBehavior
	// for example: a minion is CheckForDeath()'d and while not dead, is low enough on HP to quit.  
	//   This could just as easily be implemented as a 'turn'.  Instead of doing one of the Four Actions, the minion flees or heals or whatever.
	//   Since the player doesn't get a chance to respond to things, that's probably more fair.
	// so we'll put it in Character
	
	public IEnumerator CheckForDeath()
	{
		if( stats.HP <= 0)
		{
			notifier.ShowActionLabel( name + " has died!");
			isDead = true;
			sfx.PlayOneShot( deathSfx);
			yield return helper.StartCoroutine( DeathAnimation()); // show a graphic or animation
		}
		else {
			if( isDefending) // if the character being attacked is defending, keep them in the defending animation
				yield return helper.StartCoroutine( DefendAnimation() );
			else 			 // otherwise, show the 'struck' animation and then return to the normal idle animation
			{
				yield return helper.StartCoroutine( StruckAnimation()); // show a 'hurt' animation here
				yield return new WaitForSeconds(1);
				yield return helper.StartCoroutine( IdleAnimation() );  // then go back to idle
			}
		}
	}

}
