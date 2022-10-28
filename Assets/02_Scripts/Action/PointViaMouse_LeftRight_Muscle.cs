﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointViaMouse_LeftRight_Muscle : MonoBehaviour
{
    /* we need this since slot pos are shuffeld but we want Slot[0] to be at top */
  // private Vector3[] SlotsPositions;


  [Range (-180.0f, 180.0f)]
  public float AdjustAngle = 0.0f;

  public bool leftSide = true;
  public float offset = 50f;

  public bool hasLaser = true;
  public bool laserAlwaysOn = true;
  public float laserActiveTime = .4f;


  public GameObject laserEnd;
  public GameObject laserStart;
  private LineRenderer laser;
  private Camera maincam;

  private CatchSlots_A19_multiple catchScript;
  private bool hasCatchScript = true;

  void Awake()
  {

    // catchScript = gameObject.GetComponent<CatchSlots_A19_multiple>();
    // if (catchScript != null) {
    //   hasCatchScript = true;
    // }
  }
  

  // Use this for initialization
  void Start () {

   maincam = Camera.main;
   if (hasLaser) {
      laser = transform.GetComponent<LineRenderer>();
      if (null==laser) { hasLaser = false; }
      else {
        laser.enabled=laserAlwaysOn;
    }
} 
}


void PointLaser () {
    //Output the current screen window width in the console
    Vector2 mouseOnScreen = (Vector2) maincam.ScreenToWorldPoint(Input.mousePosition);
    Vector3 originPosition = transform.position;
   
   float angleToMouse = AngleBetweenTwoPoints(originPosition, mouseOnScreen);
   transform.rotation = Quaternion.Euler (new Vector3(0f,0f,angleToMouse + AdjustAngle));
}

float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
 return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
}

void ShowLaser() {
      laser.enabled = true;
      Vector3 dir = (laserEnd.transform.position - laserStart.transform.position) * 100f; // make sure this is the same as in CatchSlots
      laser.SetPosition( 0, laserStart.transform.position );
      laser.SetPosition( 1, dir );
}

  // Update is called once per frame
void Update () {

  if (leftSide && Input.mousePosition.x > (Screen.width/2 - offset)) return;
  if (!leftSide && Input.mousePosition.x < (Screen.width/2 + offset)) return;
  PointLaser();
  if (!hasLaser) return;
  if (laserAlwaysOn) {
    ShowLaser();
  }  else if (Input.GetMouseButtonUp(0)){
    /* see if there is a current tracktor beam*/
    // Debug.Log(hasCatchScript);
    // Debug.Log(catchScript.isMovingOne);
    // if (hasCatchScript && catchScript.isMoving==false) {
      ShowLaser();
      StartCoroutine("LaserOff",laserActiveTime);
    // }
  }
    // }
}

  // void ShowLaser() {

  // }
     /* this one requires that we have less object than slots*/
private IEnumerator LaserOff(float time) {
  yield return new WaitForSeconds(time);
  laser.enabled = false;
}

}
