using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

	// Use this for initialization
	void Start () {
		//GameObject.DontDestroyOnLoad( gameObject);	// don't allow ourselves to be deleted
	}

	public void Play( AudioClip music)
	{
		// don't play twice, because that makes it restart
		if( !audio.isPlaying)
		{
			audio.clip = music;
			audio.Play();
		}
	}

	
	static public BackgroundMusic GetSingleton()
	{
		GameObject ret = GameObject.Find ("Background Music Singleton");
		if(ret == null)
		{
			Debug.Log("Didn't find any background music; creating one from scratch...");
			GameObject go = new GameObject( "Background Music Singleton");
			BackgroundMusic bm = go.AddComponent<BackgroundMusic>();
			AudioSource source = go.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.loop = true;

			//bm.SetDefaults();
			return bm;
		}
		return ret.GetComponent<BackgroundMusic>();
	}
}
