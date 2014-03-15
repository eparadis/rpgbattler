using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoroTest : MonoBehaviour {

	List<Square> squares;

	// Use this for initialization
	void Start () {
		squares = new List<Square>();
		squares.Add( new Square( Color.red, -2f ));
		squares.Add( new Square( Color.blue, 0  ));
		squares.Add( new Square( Color.green, 2f ));

		StartCoroutine( MakeSquaresDance() );
	}

	IEnumerator MakeSquaresDance()
	{
		foreach( Square s in squares)
		{
			yield return StartCoroutine( s.myDance.DoDance() );
			yield return new WaitForSeconds( 1.0f);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}

public class Square {

	public GameObject gfx;
	public Dance myDance;
	public Helper helper;

	public Square( Color c, float offset)
	{
		gfx = GameObject.CreatePrimitive( PrimitiveType.Cube);
		gfx.renderer.material.color = c;
		gfx.transform.Translate( offset, 0, 0 );
		myDance = new Dance( Dance.DanceType.Spin, this );
		helper = gfx.AddComponent<Helper>();
	}

}

public class Dance {

	public enum DanceType {
		Spin, Flip, Jiggle
	};

	DanceType danceType;
	Square self;

	public Dance( DanceType dt, Square s )
	{
		danceType = dt;
		self = s;
	}
	
	public IEnumerator DoDance()
	{
		Debug.Log ("inside DoDance");
		if(danceType == DanceType.Spin)
		{
			yield return self.gfx.GetComponent<Helper>().StartCoroutine( SpinDance( 100f) );
		} else {
			Debug.LogError( "Square doesn't know that dance!");
			yield return null;
		}
	}


	IEnumerator SpinDance( float speed)
	{
		Vector3 rot;
		do
		{
			rot = self.gfx.transform.rotation.eulerAngles;
			self.gfx.transform.Rotate(Vector3.up, Time.deltaTime * speed);
			yield return null;
		} while( rot.y < 350f);
	}
}

