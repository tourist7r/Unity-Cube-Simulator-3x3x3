using UnityEngine;
using System.Collections;

public class LightningGenerator : MonoBehaviour {

	public Lightning LightningPrefab;
	public GameObject EndPoint;
	public int NumToSpawn = 5;
	public bool Spawn = true;

	void Update () {
		if (Spawn) {
			Spawn = false;
			for (int i = 0; i < NumToSpawn; i++) {
				GameObject litGO = Instantiate (LightningPrefab.gameObject, transform.position, Quaternion.identity) as GameObject;
				if (EndPoint != null) {
					Lightning lit = litGO.GetComponent<Lightning> ();
					lit.EndPoint = EndPoint.transform;
				}
			}
		}
	}
}