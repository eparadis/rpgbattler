using UnityEngine;
using System.Collections;

public class ClickToJump : MonoBehaviour {

	Animator an;

	// Use this for initialization
	void Start () {
		an = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			an.SetTrigger("doJump");
		}
	
		//if(an.GetCurrentAnimatorStateInfo(0).IsName("idle")
	}
}
