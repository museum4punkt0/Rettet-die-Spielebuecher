using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPK;

/*
 a combination of ToggleLookUp and SimpleMouseDragScripts
 in order to be able to integrate MouseDrag even when the Gameobject 
 follows an other GO.
 */
public class HandleDragAndActivation : MonoBehaviour
{
  
  [Tooltip("Other Gameobject with a LookAt Script which gonna be activated")]
  public GameObject other;
  LookAt otherLookAt;

  [Tooltip("Collider triggering the activation")]
  public Collider2D activationZoneCol;
  public Collider2D errorZoneCol;

  private Vector3 startPosition;
  
  // float starttime;

  [Tooltip("when Pin should follow a GO transform, than set this here, or keep empty")]
  public Transform followTransform;
  
  bool inPlace = false; // is it at correct place
  bool follow = false;

  [Tooltip("Should the Dropzone Sprite be hidden when pin at place")]
  public bool hideDropZoneSprite;
  [Tooltip("Should the Sprite of this PIN be hidden when at place?")]
  public bool hideSelfSprite = true;


  /* Mouse Dragging*/
  bool _dragged = false;

  Collider2D col;
  Camera Maincam;
  public float dragDistanceFromMouse = 10.0f;


  /* Materials */
  public Material defaultMat;
  public Material mouseOverMat;
  public Material draggingMat;
  public Material inPlaceMat;

  SpriteRenderer spriteRenderer;
 

  void Start () {
    if (!other) throw new System.NullReferenceException("Add Other GO");
    if (!activationZoneCol) throw new System.NullReferenceException("Add GO with Collider2d ");

    startPosition = transform.position;
    
    // otherCol = other.GetComponent<Collider2D>();
    // if (!otherCol)  throw new System.NullReferenceException("Add Collider to otherCol");

    otherLookAt = other.GetComponent<LookAt>();

   if (!otherLookAt) throw new System.NullReferenceException("Other Object needs a Script 'Look at'");

   /* Mouse Dragging */
     Maincam = Camera.main;
    col = transform.gameObject.GetComponent<Collider2D>();
    if (!Maincam.orthographic)
    {
        throw new System.ArgumentException("Set Maincam projection to orthographic ", "SimpleMouseDrag");
    }

    spriteRenderer = GetComponent<SpriteRenderer>();
    spriteRenderer.material = defaultMat;
  }

  void Update()
  {
    if (follow){
      transform.position = followTransform.position;
    }

    /* Dragging */
    var mouseInWorldPos = Maincam.ScreenToWorldPoint(Input.mousePosition);
    
    bool mouseOver = col.OverlapPoint(mouseInWorldPos);
    if (Input.GetMouseButtonDown(0)) {
      if (!mouseOver) {
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
       spriteRenderer.material = draggingMat;
    } else if (inPlace) {
      spriteRenderer.material = inPlaceMat;
    } else if (mouseOver) {
       spriteRenderer.material = mouseOverMat;
    } else {
      spriteRenderer.material = defaultMat;
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
    if (errorZoneCol == o) {
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
