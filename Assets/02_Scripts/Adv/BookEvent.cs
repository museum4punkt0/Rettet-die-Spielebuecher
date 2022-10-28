using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookEvent : MonoBehaviour
{

    Animator animator;
    // public Collider playerCol;
      /* tracking time*/
    float starttime;

    // Start is called before the first frame update
    void Start()
    {
      animator = GetComponent<Animator>();
      animator.SetBool("open", false);
      // starttime = Time.time; 
    }

    /* this is triggered by Animation  Events */
    public void AnimationEvent (string message) {
       Messenger<string>.Broadcast("BOOK_OPEN", message);
       // Debug.Log("BOOK_OPEN " +  message);
    }

}
