using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* the goal of this class is to have something you can drag to an empty game object,
 *   then set a few assets very quickly, and have it build a complete set of animations 
 *   and whatever is required.
 * 
 * this could be built up at run time, or in the editor.  I don't know which yet.
 */

public class SpriteCharacterBuilder : MonoBehaviour {

	public Sprite[] frames;
	public bool facingLeft;

	// Use this for initialization
	void Start () {
		SetupSpriteRenderer();
		SetupAnimator();
		SetupParticleSystem();

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SetupSpriteRenderer()
	{
		//SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
		//sr.sprite = frames[0];

	}

	void SetupAnimator()
	{
		// we'll be using our own custom animator

		SpriteAnimator sa = GetComponent<SpriteAnimator>();
		if( sa == null)
			return;

		if( facingLeft)
			sa.FlipTo(-1);

		// the idle animation
		SpriteAnimator.Animation idle = new SpriteAnimator.Animation();
		idle.name = "idle";
		idle.frames = new Sprite[2];
		idle.frames[0] = frames[0];
		idle.frames[1] = frames[1];
		idle.fps = 2;
		idle.triggers = new SpriteAnimator.AnimationTrigger[0];

		SpriteAnimator.Animation stab = new SpriteAnimator.Animation();
		stab.name = "stab";
		stab.frames = new Sprite[2];
		stab.frames[0] = frames[2];
		stab.frames[1] = frames[3];
		stab.fps = 2;
		stab.triggers = new SpriteAnimator.AnimationTrigger[0];

		SpriteAnimator.Animation cast = new SpriteAnimator.Animation();
		cast.name = "cast";
		cast.frames = new Sprite[2];
		cast.frames[0] = frames[1];
		cast.frames[1] = frames[4];
		cast.fps = 2;
		cast.triggers = new SpriteAnimator.AnimationTrigger[0];

		SpriteAnimator.Animation die = new SpriteAnimator.Animation();
		die.name = "die";
		die.frames = new Sprite[1];
		die.frames[0] = frames[7];
		//die.frames[1] = frames[4];
		die.fps = 2;
		die.triggers = new SpriteAnimator.AnimationTrigger[0];

		SpriteAnimator.Animation struck = new SpriteAnimator.Animation();
		struck.name = "struck";
		struck.frames = new Sprite[2];
		struck.frames[0] = frames[6];
		struck.frames[1] = frames[1];
		struck.fps = 2;
		struck.triggers = new SpriteAnimator.AnimationTrigger[0];

		SpriteAnimator.Animation defend = new SpriteAnimator.Animation();
		defend.name = "defend";
		defend.frames = new Sprite[2];
		defend.frames[0] = frames[5];
		defend.frames[1] = frames[8];
		defend.fps = 2;
		defend.triggers = new SpriteAnimator.AnimationTrigger[0];

		List<SpriteAnimator.Animation> animations = new List<SpriteAnimator.Animation>();
		animations.Add(idle);
		animations.Add(stab);
		animations.Add(cast);
		animations.Add(die);
		animations.Add(struck);
		animations.Add(defend);
		sa.animations = animations.ToArray();	// this is kind of a waste but it makes for more sane syntax

		sa.Play("idle");

	}

	void SetupParticleSystem()
	{
		// the particle system is a component on a gameobject parented to this one

		/*ParticleSystem ps = gameObject.AddComponent<ParticleSystem>();

		//ps.duration = 5f;
		ps.loop = false;
		ps.startDelay = 0;
		ps.startLifetime = 2;
		ps.startSpeed = -1;
		ps.startSize = 1;
		ps.startRotation = 0;
		ps.startColor = Color.white;
		ps.gravityModifier = 0;
		//ps.inheritVelocity = 0f;
		ps.simulationSpace = ParticleSystemSimulationSpace.Local;
		ps.playOnAwake = false;
		ps.maxParticles = 1000;

		ps.emissionRate = 10; */


	}
}
