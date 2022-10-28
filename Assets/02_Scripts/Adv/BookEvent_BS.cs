using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookEvent_BS : MonoBehaviour
{

    Animator animator;
    public AudioClip clipBookOpen;
    private AudioSource audioBookOpen;

    // public Collider playerCol;
      /* tracking time*/
    float starttime;

    // Start is called before the first frame update
    void Start()
    {
      audioBookOpen = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
      audioBookOpen.clip = clipBookOpen;
      audioBookOpen.playOnAwake = false;
      audioBookOpen.volume = 0.5f;
      animator = GetComponent<Animator>();
      animator.SetBool("open", false);
      // starttime = Time.time;
    }

    /* this is triggered by Animation  Events */
    public void AnimationEvent (string message) {
       audioBookOpen.Play();
       Messenger<string>.Broadcast("BOOK_OPEN", message);
       // Debug.Log("BOOK_OPEN " +  message);
    }

}
