using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
// drag all 2d object of set layer (with collider) via mouse drag
// 
// Make sure that camera is orthographic !!
// 
/// </summary>

public class SimpleMouseDrag : MonoBehaviour {

  public LayerMask draggingLayer;

  // public Color color = Color.red;
  // public bool DrawDragLine = true;
  // 
  private bool _dragged = false;
  Collider2D col;
  Transform parentTrans;
  Camera Maincam;
  public float dragDistanceFromMouse = 10.0f;

  private CircleCollider2D circleCollider2D;


  // Use this for initialization
	void Start () {
    Maincam = Camera.main;
    circleCollider2D = GetComponent<CircleCollider2D>();
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
    // /* 
    //  * release mouse over it. 
    //  * 
    //  */
    // if (Input.GetMouseButtonUp(0)) {
    //    if (!col.OverlapPoint(mouseInWorldPos)) {
    //     return;
    //   }
    //   transform.parent = parentTrans;
    // }
    if (Input.GetMouseButtonDown(0)) {
      parentTrans = transform.parent;
      // Fetch the FIRST  collider under mouse.
      // NOTE: We could do this for multiple colliders.
     
      if (!col.OverlapPoint(mouseInWorldPos)) {
        return;
      }
      _dragged = true;
      circleCollider2D.enabled = false;
    }
    else if (Input.GetMouseButtonUp(0)) {
      _dragged = false;
      // if (switchOffByClickautoSimulation) {
      // Physics2D.autoSimulation = true;
      // switchOffByClickautoSimulation = false;
      // }
      circleCollider2D.enabled = true;

    }
    
    if (_dragged) {
       Vector3 newPos = new Vector3(mouseInWorldPos.x,mouseInWorldPos.y,-4.0f);
       transform.position = newPos;
       
    }

     /* unsetting drag when mouse has moved too fast */
    Vector2 offset = mouseInWorldPos - transform.position;
    float sqrLen = offset.sqrMagnitude;
    // square the distance we compare with
    if (sqrLen > dragDistanceFromMouse * dragDistanceFromMouse)
    {
      _dragged = false;
      // print("mouse too far away for dragging / mouse too fast !");
    }
	 }
}
