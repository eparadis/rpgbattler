using UnityEngine;
using System.Collections;

public class BattleConfig : MonoBehaviour {
	
	public int level;	// level is one-based.  the first level is '1'
	public int playerCharacter;
	public CharacterStats PCStats;
	
	// Use this for initialization; remember that this is called on the start of a level load, even if we're coming in from a previous scene
	void Start () {
		GameObject.DontDestroyOnLoad( gameObject);	// don't allow ourselves to be deleted
	}

	private void SetDefaults()
	{
		if( Application.isEditor)	// if we're getting this from inside the editor, chances are we're play testing and just want some values here
		{
			level = 2;	// number of badguys
			playerCharacter = 1;
			PCStats = new CharacterStats( 2, 2, 2, 2);
		} else {
			level = 1;
			playerCharacter = 1;
			PCStats = new CharacterStats(0, 0, 0, 0);
		}
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
