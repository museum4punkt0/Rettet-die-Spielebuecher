using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
// drag object via mouse
// Make sure that camera is orthographic !!
// 
/// </summary>

namespace SPK {
  
public class SimpleMouseDragNoLayer : MonoBehaviour {

  public bool _dragged = false;
  Collider2D col;
  Camera Maincam;
  public float dragDistanceFromMouse = 10.0f;



	// Use this for initialization
	void Start () {
    Maincam = Camera.main;
    col = transform.gameObject.GetComponent<Collider2D>();
    if (!Maincam.orthographic)
    {
        throw new System.ArgumentException("Set Maincam projection to orthographic ", "SimpleMouseDrag");
    }
	}

	
	// Update is called once per frame
	void Update () {
	// 	 // Calculate the world position for the mouse.
    var mouseInWorldPos = Maincam.ScreenToWorldPoint(Input.mousePosition);
   
    if (Input.GetMouseButtonDown(0)) {
      if (!col.OverlapPoint(mouseInWorldPos)) {
        return;
      }
      _dragged = true;
    
    }
    else if (Input.GetMouseButtonUp(0)) {
      _dragged = false;
    }
    
    if (_dragged) {
       Vector3 newPos = new Vector3(mouseInWorldPos.x,mouseInWorldPos.y,transform.position.z);
       transform.position = newPos;
    }

     /* unsetting drag when mouse has moved too fast */
    Vector2 offset = mouseInWorldPos - transform.position;
    float sqrLen = offset.sqrMagnitude;
    // square the distance we compare with
    if (sqrLen > dragDistanceFromMouse * dragDistanceFromMouse)
    {
      _dragged = false;
    }
	 }
}
}
