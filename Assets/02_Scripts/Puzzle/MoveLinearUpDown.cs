using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLinearUpDown : MonoBehaviour {


  public int direction = -1;
  int formerDir = -1;
  [Range(0.001f,0.5f)]
  public float Speed = 0.004f;

  public float changeDirDelay = 0.1f;
  bool changDir = false;
  bool reset = false;

  public Collider2D restrictArea;

  Vector3 defaultPos;
	// Use this for initialization
	void Start () {
		defaultPos = transform.position;
	}
	
  public void ResetToDefault() {
    reset = true;
    StartCoroutine("resetMe", changeDirDelay);
  }

	// Update is called once per frame
	void Update () {
    if (changDir || reset) return;
    
    float newY = transform.position.y - (Speed * direction);
    transform.position = new Vector2(transform.position.x,newY);

		if (!restrictArea.OverlapPoint(transform.position) || formerDir != direction) {
      changDir = true;
      StartCoroutine("changeDir", changeDirDelay);
      return;
  	}
    /* pause when dir change*/
    // if (direction == formerDir) {
    //    direction = direction * -1;
    // } 
  }

   private IEnumerator resetMe ( float waittime) {
      yield return new WaitForSeconds(waittime);
      transform.position = defaultPos;
      direction = -1;
      formerDir = -1;
      reset = false;
  }

  private IEnumerator changeDir ( float waittime) {
      yield return new WaitForSeconds(waittime);
       direction = direction * -1;
       formerDir = direction;
       changDir = false;
  }
}
