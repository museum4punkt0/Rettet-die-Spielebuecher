using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPK;
/* 
apply this to a GO with a Collider 
 set the other GO with a Collider and a Script LookAt 
 */
public class ToggleLookUp : MonoBehaviour
{  
  
  [Tooltip("Other Gameobject with a LookAt Script which gonna be activated")]
  public GameObject other;
  LookAt otherLookAt;

  [Tooltip("Collider triggering the activation")]
  public Collider2D activationZoneCol;
  public Collider2D errorZoneCol;

  private Vector3 startPosition;

  // float starttime;

  [Tooltip("When Pin should follow a GO transform, than set this here, or keep empty")]
  public Transform followTransform;
  
  bool follow = false;

  [Tooltip("Should the Dropzone Sprite be hidden when pin at place")]
  public bool hideDropZoneSprite;
  [Tooltip("Should the Sprite of this PIN be hidden when at place?")]
  public bool hideSelfSprite = true;
 

  void Start () {
   

    if (!other) throw new System.NullReferenceException("Add Other GO");
    if (!activationZoneCol) throw new System.NullReferenceException("Add GO with Collider2d ");
    
    startPosition = transform.position;

    // otherCol = other.GetComponent<Collider2D>();
    // if (!otherCol)  throw new System.NullReferenceException("Add Collider to otherCol");

    otherLookAt = other.GetComponent<LookAt>();

   if (!otherLookAt) throw new System.NullReferenceException("Other Object needs a Script 'Look at'");

  }

  void Update()
  {
    if (follow){
      transform.position = followTransform.position;
    }

  }
  
  // void OnTriggerStay(Collider2D o) 
  // {
  //   if (activationZoneCol == o) {
  //     otherLookAt.enabled = true;
  //      //  var completetime = Time.time - starttime;
  //      // Messenger<float>.Broadcast("QUEST_COMPLETED", completetime);
  //   }
  // }
  // 
  
  void HideDropZoneSprite(bool OnOff) {
     SpriteRenderer sr = activationZoneCol.gameObject.GetComponent<SpriteRenderer>();
      if (sr != null) {
          sr.enabled = OnOff;
      }
  }

   void HideSelfSprite(bool OnOff) {
     SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
      if (sr != null) {
          sr.enabled = OnOff;
      }
  }

 /* Pin at Place*/
  void OnTriggerEnter2D(Collider2D o) {
      if (errorZoneCol == o && !follow) {
          transform.position = startPosition;
      }
      
    if (activationZoneCol == o) {
      otherLookAt.enabled = true;
      // Debug.Log("PIN_AT_PLACE " + transform.gameObject.name);
      Messenger<string>.Broadcast("PIN_AT_PLACE", transform.gameObject.name );
      if (hideSelfSprite) {
        HideSelfSprite(false);
      }
      if (followTransform != null) {
        follow = true;
      }
      if (hideDropZoneSprite) {
       HideDropZoneSprite(false);
      }
       //  var completetime = Time.time - starttime;
       // Messenger<float>.Broadcast("QUEST_COMPLETED", completetime);
    }
  }


  void OnTriggerExit2D(Collider2D o) {
    if (activationZoneCol == o) {
      otherLookAt.enabled = false;
       Messenger<string>.Broadcast("PIN_AWAY_PLACE", transform.gameObject.name );
      follow = false;
      if (hideDropZoneSprite) {
       HideDropZoneSprite(true);
      }
       if (hideSelfSprite) {
        HideSelfSprite(true);
      }
    }
  }

}
