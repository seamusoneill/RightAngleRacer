using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanRightToLeft : MonoBehaviour {
	int count = 180;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		transform.Rotate(0,count *Time.deltaTime,0);
		count--;
	}
}
