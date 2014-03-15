using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehavior { //: MonoBehaviour {

	/* the result of an action will be
	 * doPhysAttack, target
	 * doMagAttack, target
	 * doDefend, null
	 * doHeal, target
	 */

	public enum ActionType {
		PhysAttack, MagAttack, Defend, Heal
	};

	public enum Architype {
		Healer, Mage, Brawler, Minion, Boss, Player
	};

	private Character self;

	public EnemyBehavior( Architype ar, Character ch )
	{
		self = ch;
		switch ( ar )
		{
		case Architype.Mage:

			break;
		default:
			break;
		}
	}

	public delegate IEnumerator TakeTurn( List<Character> allChars);


	public IEnumerator LegacyPhysAttackTurn( List<Character> allChars )
	{
		// this is what we're trying to replace

		Character target = FindStrongestPC( allChars);

		if( target != null) // if there are no living characters, just skip this NPC's turn
		{
			string resultString = self.PhysicalAttack( target );	// do a physical attack on the first PC in the list
			yield return self.helper.StartCoroutine( self.notifier.ShowActionLabel( "Attack " + resultString ) );
			yield return self.helper.StartCoroutine(self.IdleAnimation() );
			yield return self.helper.StartCoroutine(self.ApproachTargetAnimation( target));

			// sfx is a game-global singleton; its design is already set; i just need to get a reference to it; only the Sequencing know about it at
			//   the moment, but all the various Behaviors are going to need to do something about it.
			// Unity would more or less have us attach a AudioSource to every GameObject (in this case, the self.gfx), but then centralized control becomes difficult
			// I decided to go with 'the Unity way'.  the existing SfxManager will stick around for doing menu sound FX and other sounds not associated with a Character.
			self.sfx.PlayOneShot(self.attackSfx); 

			yield return self.helper.StartCoroutine(self.StabAnimation() );
			yield return self.helper.StartCoroutine(target.behavior.CheckForDeath( target) );
			yield return self.helper.StartCoroutine(self.IdleAnimation() );
			yield return self.helper.StartCoroutine(self.ReturnHomeAnimation());
		}
	}

	public IEnumerator LegacyHealTurn( List<Character> allChars)
	{
		Character target = FindWeakestNPC( allChars);
		string resultString = self.CastHeal( target);	// cast 'heal' on the first non-PC in the list
		self.sfx.PlayOneShot(self.healSfx);
		yield return self.helper.StartCoroutine(self.notifier.ShowActionLabel( "Heal " + resultString));
		yield return self.helper.StartCoroutine(self.CastAnimation());
		yield return self.helper.StartCoroutine(self.ShootSparklies( Color.green ) ); 
		yield return self.helper.StartCoroutine(target.AttractSparklies( Color.green ) );
		yield return self.helper.StartCoroutine(self.IdleAnimation() );
	}

	// this is a little wierd
	// on one hand, this could be part of Character, as its really only messing with Character stuff - target.CheckForDeath()
	// on the other hand, i could see how a character's response (maybe casting a last minute save, or running away) would be a 'behavior'
	//   and this should then go into EnemyBehavior
	// for example: a minion is CheckForDeath()'d and while not dead, is low enough on HP to quit.  
	//   This could just as easily be implemented as a 'turn'.  Instead of doing one of the Four Actions, the minion flees or heals or whatever.
	//   Since the player doesn't get a chance to respond to things, that's probably more fair.
	// so we'll put it in Character
	// of course, you can't start coroutines here because Character doesn't derive from MonoBehavior
	// uuugggggh back it goes into EnemyBehavior
	
	public IEnumerator CheckForDeath( Character ch)
	{
		if( ch.stats.HP <= 0)
		{
			ch.notifier.ShowActionLabel(ch. name + " has died!");
			ch.isDead = true;
			ch.sfx.PlayOneShot( ch.deathSfx);
			yield return self.helper.StartCoroutine( ch.DeathAnimation()); // show a graphic or animation
		}
		else 
			yield return self.helper.StartCoroutine( ch.StruckAnimation()); // show a 'hurt' animation here
		//yield return StartCoroutine(ch.IdleAnimation() );  // then go back to idle
	}


	/* ********** */

	// return the NPC which has the least amount of HP
	Character FindWeakestNPC( List<Character> charList)
	{
		Character target = charList.Find ( delegate( Character z)
		                                  {     return !z.isPC && !z.isDead;    } ) ;	// TODO this Find is wrong; it just finds the first. not the weakest
		if( target == null)
			Debug.LogWarning( "FindWeakestNPC couldn't find a target");
		return target;
	}

	// return the PC which has the most amount of HP
	Character FindStrongestPC( List<Character> charList)
	{
		Character target = charList.Find ( delegate( Character z)
		                                  {     return z.isPC && !z.isDead;    } ) ;	// TODO this Find is wrong; it just finds the first. not the strongest
		if( target == null)
			Debug.LogWarning( "FindStrongestPC couldn't find a target");
		return target;
	}

	// return a random PC
	Character RandomPC( List<Character> chars)
	{
		return null; // TODO
	}

	// return a random NPC
	Character RandomNPC( List<Character> chars)
	{
		return null; // TODO
	}

	/*void HealerArchitype( out ActionType action, out Character target )
	{
		// never attacks
		// often heals weak allies
		// occasionally defends
		action = QuadRandomPicker( 0, .25f, 0, .75f);
		if( action == ActionType.Heal)
			target = FindWeakestNPC();
		else
			target = null;	// defending has no target
	}

	void MageArchitype( out ActionType action, out Character target )
	{
		// always magic attacks
		// never heals, phys attacks, or defends
		//QuadRandomPicker(0, 0, 1, 0); 
		action = ActionType.MagAttack;
		target = RandomPC ();
	}*/

	void BrawlerArchitype()
	{
		// never heals or magic attacks
		// defends if low on health
		// otherwise phys attack
	}

	void MinionArchitype()
	{
		// occasionally attacks
		// heals boss
		// occasionally defends
	}

	void BossArchitype()
	{
		// ignores allies
		// attacks weakest enemy
	}

	ActionType QuadRandomPicker( float physAttackChance, float defendChance, float magAttackChance, float healChance)
	{
		// each argument is 0..1, and all should add up to 1
		if( Mathf.Abs( physAttackChance + defendChance + magAttackChance + healChance - 1f) > .001f)
			Debug.LogWarning( "invalid probabilities passed to QuadRandomPicker");
		// pick a random number between 0..1
		float x = Random.Range(0, 1f);
		// find out which category it lands in
		if( x < physAttackChance)
			return ActionType.PhysAttack;
		if( x < physAttackChance + defendChance)
			return ActionType.Defend;
		if( x < physAttackChance + defendChance + magAttackChance)
			return ActionType.MagAttack;
		else
			return ActionType.Heal;
	}
}
