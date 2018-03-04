/*
 * This class is used in order to manage after reset effects...etc.
 */

using UnityEngine;
using System.Collections;

public class Reset : MonoBehaviour {

    public GameObject cube;
	//variables that will be kept after scene reload
	public bool trailEnabled = false;
	public bool pistonEnabled = false;
	public int comboSelectedIndex = 6;
	//Needs to be static.
	private static bool spawned = false;
	void Awake(){
		if(spawned == false){
			spawned = true;		
			DontDestroyOnLoad(gameObject);
		}else{
			DestroyImmediate(gameObject); //This deletes the new object/s that you
			// mentioned were being created
		}
	}
}