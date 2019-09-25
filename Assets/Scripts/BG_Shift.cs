using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Shift : MonoBehaviour
{
	// this is a comment
	public float counter = 0;
	public float amp = 0.005f;
	// Start is called before the first frame update
	void Start(){

	}

	// Update is called once per frame
	void Update(){
		float z = -(float)System.Math.Cos(counter) / 2;
		transform.position = new Vector3(4.29f, 3.79f, z * 8 + 5);
		counter += 0.002f;
	}

}
