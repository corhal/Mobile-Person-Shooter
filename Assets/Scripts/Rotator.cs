using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
		
	// Update is called once per frame
	void Update () {
		if(Input.GetButton ("Fire1")) {
			Ray shootRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			transform.LookAt (shootRay.GetPoint(10.0f));
		}
	}
}
