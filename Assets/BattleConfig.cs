using UnityEngine;
using System.Collections;

public class BattleConfig : MonoBehaviour {
	
	public int level;
	public int playerCharacter;
	
	// Use this for initialization; remember that this is called on the start of a level load, even if we're coming in from a previous scene
	void Start () {
		GameObject.DontDestroyOnLoad( gameObject);	// don't allow ourselves to be deleted
	}

	public void SetDefaults()
	{
		level = 0;
		playerCharacter = 0;
	}

	static public BattleConfig GetSingleton()
	{
		GameObject ret = GameObject.Find ("Battle Config Singleton");
		if(ret == null)
		{
			Debug.Log("Didn't find a battle configuration; creating one from scratch...");
			GameObject go = new GameObject( "Battle Config Singleton");
			BattleConfig bc = go.AddComponent<BattleConfig>();
			bc.SetDefaults();
			return bc;
		}
		return ret.GetComponent<BattleConfig>();
	}
}
