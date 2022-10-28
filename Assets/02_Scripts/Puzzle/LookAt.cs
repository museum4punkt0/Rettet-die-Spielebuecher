using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SPK {

  public class LookAt : MonoBehaviour {

    public Transform target;
    [Range (-180.0f, 180.0f)]
    public float AdjustAngle = 0.0f;

    [Range(0.01f,1f)]
    public float smooth = 0.5f;

    [Tooltip("Check this if - when active - the puzzle is solved")]
    public bool PuzzleSolver = false;

    void OnEnable() {
      // if (PuzzleSolver) {
      //   // Debug.Log("ENABLEEEEEE");
      //   Messenger.Broadcast("PUZZLE_SOLVED");
      // }
    }
  


  	// Update is called once per frame
  	void Update () {
       if (target == null) return;
       Vector3 dir = target.transform.position - transform.position;
       float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
       transform.rotation = Quaternion.Lerp(
        transform.rotation,
        Quaternion.AngleAxis(angle - AdjustAngle, Vector3.forward), 
        smooth);
     
       // transform.rotation = Quaternion.AngleAxis(angle - AdjustAngle, Vector3.forward);
  	}

  }
}
