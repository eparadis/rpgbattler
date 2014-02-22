using UnityEngine;
using System.Collections;

public class KeyboardControl : MonoBehaviour {

	public GameObject character;
	public float walkSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.D))
			transform.Translate( new Vector3( walkSpeed * Time.deltaTime, 0, 0));
		if(Input.GetKey(KeyCode.A))
			transform.Translate( new Vector3( -walkSpeed * Time.deltaTime, 0, 0));
		if(Input.GetKey(KeyCode.W))
			transform.Translate( new Vector3( 0, walkSpeed * Time.deltaTime, 0));
		if(Input.GetKey(KeyCode.S))
			transform.Translate( new Vector3( 0, -walkSpeed * Time.deltaTime, 0));
		if(Input.GetKeyDown(KeyCode.Space) && !character.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base Layer.all frames") )
		{
			character.GetComponent<Animator>().SetTrigger("doJump");
		}

	}
}
