using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyCharacterInteraktion : MonoBehaviour
{


    public GraphUpdateObject guo;
  


    public Animator animator;
    // public Collider playerCol;
      /* tracking time*/
    // float starttime;

    // Start is called before the first frame update
    void Start()
    {
      animator = GetComponent<Animator>();
      animator.SetBool("OnceHit", false);
      // starttime = Time.time; 

      //     // As an example, use the bounding box from the attached collider
      // Bounds bounds = GetComponent<Collider>().bounds;

      // // Set some settings
      // guo.updatePhysics = false;
      // AstarPath.active.UpdateGraphs(guo);


    }

    /* this is triggered by Animation  Events */
    public void AnimationEvent (string message) {
      // Debug.Log("Animation Event " + message);
      animator.SetBool("OnceHit", false);
    }

}
