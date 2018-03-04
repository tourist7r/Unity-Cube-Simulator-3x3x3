

/*
 * This class is reponsible for handling the background music and cube face turning sound effect.
 * BGM and SFX files can be found in the project under Audio folder.
 * Toggling the bgm tick in game will stop the current playing bgm and will play the next bgm on the next tick.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioController : MonoBehaviour {
	public bool isBGMPlaying = true;

	//BGM
	[SerializeField]
	public AudioClip[] bgm;
	private AudioSource bgmsource;
	
	//sfx
	public AudioClip rubikturnsfx;
	[HideInInspector]
	public AudioSource sfxsource;
	public bool enableSFX = true;
	public int play = 0;
	//Needs to be static.
	private static bool spawned = false;
	
	void Awake(){

		if (spawned == false) {
			spawned = true;			
			DontDestroyOnLoad (gameObject);	

		} else {
			DestroyImmediate (gameObject);
		}
	}

	void Start(){
		bgmsource = gameObject.AddComponent < AudioSource > ();
		sfxsource = gameObject.AddComponent < AudioSource > ();
		
		bgmsource.volume = 0.5f;
		bgmsource.loop = true;
		bgmsource.clip = bgm[play];

		//bgmsource.Play ();
	}
	
	public void OnBGMTogglePushed(){
		if (bgmsource.isPlaying) {
			bgmsource.Stop ();
			play++;
			play = play == bgm.Length ? 0 : play;
			isBGMPlaying = false;
		} else {
			bgmsource.clip = bgm[play];
			bgmsource.Play ();
			isBGMPlaying = true;
		}
	}

	public void OnSFXTogglePushed(){
		enableSFX = enableSFX ? false : true;
	}
}
