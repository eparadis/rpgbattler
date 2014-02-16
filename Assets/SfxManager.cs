using UnityEngine;
using System.Collections;

public class SfxManager : MonoBehaviour {

	public AudioClip menuSelect;

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

	public void Play( AudioClip sound)
	{
		audio.clip = sound;
		audio.Play();
	}
	
	
	static public SfxManager GetSingleton()
	{
		GameObject ret = GameObject.Find ("Sound FX Singleton");
		if(ret == null)
		{
			Debug.Log("Didn't find any sound effect manager; creating one from scratch...");
			GameObject go = new GameObject( "Sound FX Singleton");
			SfxManager bm = go.AddComponent<SfxManager>();
			AudioSource source = go.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.loop = false;
			
			//bm.SetDefaults();
			return bm;
		}
		return ret.GetComponent<SfxManager>();
	}
}
