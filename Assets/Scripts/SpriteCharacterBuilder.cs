using UnityEngine;
using System.Collections;

/* the goal of this class is to have something you can drag to an empty game object,
 *   then set a few assets very quickly, and have it build a complete set of animations 
 *   and whatever is required.
 * 
 * this could be built up at run time, or in the editor.  I don't know which yet.
 */

public class SpriteCharacterBuilder : MonoBehaviour {

	public Sprite[] frames;

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
