using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBasedOnVector : MonoBehaviour {

  [Range (-180.0f, 180.0f)]
  public float AdjustAngle = 0.0f;

 

  public Transform other;
	// Use this for initialization
	void Start () {
		
	}


	// Update is called once per frame
	void Update () {
		
    Vector3 line = other.position - transform.position ;
    // Debug.DrawLine(transform.position, line);
    float rotationZ = Mathf.Atan2(line.y, line.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ+AdjustAngle);
	}
}
