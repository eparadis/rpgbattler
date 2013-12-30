using UnityEngine;
using System.Collections;

public class SpriteWalker : MonoBehaviour {

	private Animator animator;
	
	// Use this for initialization
	void Start()
	{
		animator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update()
	{
		
		var vertical = Input.GetAxis("Vertical");
		var horizontal = Input.GetAxis("Horizontal");
		
		if (vertical > 0)	// north
		{
			animator.SetInteger("Direction", 0);
		}
		else if (vertical < 0)	// south
		{
			animator.SetInteger("Direction", 2);
		}
		else if (horizontal < 0)	// west
		{
			animator.SetInteger("Direction", 3);
			transform.localScale = new Vector3( 1, 1, 1);	// unflip graphic
		}
		else if (horizontal > 0)	// east
		{
			animator.SetInteger("Direction", 3);	// this would be 1, but we dont have flipped graphics, and thus must do so here
			transform.localScale = new Vector3(-1, 1, 1);	// flip graphic
		}
	}
}
