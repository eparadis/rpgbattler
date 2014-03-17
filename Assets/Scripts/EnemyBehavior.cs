using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealerArchitype : EnemyBehavior {

	public override IEnumerator TakeTurn( List<Character> allChars)
	{
		yield return self.helper.StartCoroutine( HealWeakestNPC( allChars));
	}

	public HealerArchitype( Character ch) : base( ch)	// does EnemyBehavior constructor first
	{
		// healer specific stuff here, like if we need to keep track of what we did last turn
	}
}

// if you just want to make a quick behavior that isn't named, or is data driven, use this.
public class SimpleArchitype : EnemyBehavior {

	float physAttackChance, defendChance, magAttackChance, healChance; 

	public override IEnumerator TakeTurn( List<Character> allChars)
	{
		switch( QuadRandomPicker( physAttackChance, defendChance, magAttackChance, healChance) )
		{
		default:
		case ActionType.Defend:
			yield return self.helper.StartCoroutine(Defend( allChars));
			break;
		case ActionType.Heal:
			yield return self.helper.StartCoroutine(HealWeakestNPC( allChars));
			break;
		case ActionType.MagAttack:
			yield return self.helper.StartCoroutine(MagAttackStrongestPC( allChars));
			break;
		case ActionType.PhysAttack:
			yield return self.helper.StartCoroutine(PhysAttackStrongestPC( allChars));
			break;
		}
	}

	public SimpleArchitype( Character ch, float physAttackChance, float defendChance, float magAttackChance, float healChance) : base( ch)
	{
		this.physAttackChance = physAttackChance;
		this.defendChance = defendChance;
		this.magAttackChance = magAttackChance;
		this.healChance = healChance;
	}
}


public abstract class EnemyBehavior { //: MonoBehaviour {

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

	protected Character self;

	protected EnemyBehavior( Character ch )
	{
		self = ch;
	}

	abstract public IEnumerator TakeTurn( List<Character> allChars);
	/*public IEnumerator TakeTurn( List<Character> allChars)
	{
		switch( QuadRandomPicker( 0.25f, 0.25f, 0.25f, 0.25f) )	// TODO vary these probabilites based on the enemie's architype upon setup
		{
		default:
		case ActionType.Defend:
			yield return self.helper.StartCoroutine(Defend( allChars));
			break;
		case ActionType.Heal:
			yield return self.helper.StartCoroutine(HealWeakestNPC( allChars));
			break;
		case ActionType.MagAttack:
			yield return self.helper.StartCoroutine(MagAttackStrongestPC( allChars));
			break;
		case ActionType.PhysAttack:
			yield return self.helper.StartCoroutine(PhysAttackStrongestPC( allChars));
			break;
		}
	}*/

	protected IEnumerator PhysAttackStrongestPC( List<Character> allChars )
	{
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
			yield return self.helper.StartCoroutine(target.CheckForDeath() );
			yield return self.helper.StartCoroutine(self.IdleAnimation() );
			yield return self.helper.StartCoroutine(self.ReturnHomeAnimation());
		}
	}

	protected IEnumerator HealWeakestNPC( List<Character> allChars)
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

	protected IEnumerator MagAttackStrongestPC( List<Character> allChars )
	{
		Character target = FindStrongestPC( allChars);
		string resultString = self.CastAttack( target);	// cast 'heal' on the first non-PC in the list
		self.sfx.PlayOneShot(self.magAttackSfx);
		yield return self.helper.StartCoroutine(self.notifier.ShowActionLabel( "Magic Attack " + resultString));
		yield return self.helper.StartCoroutine(self.CastAnimation());
		yield return self.helper.StartCoroutine(self.ShootSparklies( Color.red ) ); 
		yield return self.helper.StartCoroutine(self.IdleAnimation() );
		yield return self.helper.StartCoroutine(target.StruckAnimation() );
		yield return self.helper.StartCoroutine(target.AttractSparklies( Color.red ) );
		yield return self.helper.StartCoroutine(target.IdleAnimation() );
		yield return self.helper.StartCoroutine(target.CheckForDeath());
	}

	protected IEnumerator Defend( List<Character> allChars )	// this parameter is not used but we pass it... um... i don't know why
	{
		string resultString = self.Defend();
		self.sfx.PlayOneShot( self.defendSfx);
		yield return self.helper.StartCoroutine(self.notifier.ShowActionLabel(resultString));
		yield return self.helper.StartCoroutine(self.DefendAnimation());
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

	protected ActionType QuadRandomPicker( float physAttackChance, float defendChance, float magAttackChance, float healChance)
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
