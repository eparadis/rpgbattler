using UnityEngine;
using System.Collections;

public class SpriteWalker : MonoBehaviour {

	public float walkSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.LeftArrow))
		{
			transform.localScale = new Vector3(1,1,1);
		}
		if( Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Translate( -walkSpeed * Time.deltaTime, 0, 0);
		}

		if( Input.GetKeyDown(KeyCode.RightArrow))
		{
			transform.localScale = new Vector3(-1,1,1);
		}
		if( Input.GetKey( KeyCode.RightArrow))
		{
			transform.Translate( walkSpeed * Time.deltaTime, 0, 0);
		}


	}
}
