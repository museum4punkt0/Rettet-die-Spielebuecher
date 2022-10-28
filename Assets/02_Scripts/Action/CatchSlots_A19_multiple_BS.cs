using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*<summary>
 * apply on arm or other central movement.
 *</summary>*/
public class CatchSlots_A19_multiple_BS : MonoBehaviour
{

  public GameObject rayto; // where to point at
  public GameObject rayfrom; // where to point at
  // public LayerMask HitOnlyLayer; // trigger collision in this layer only
  public GameObject dropZone; // where the objects move

  /* for Beam-Moving an Object */
  public float beamSpeed = 5f;


  private float startTime;
  private Vector3 startPos;
  private GameObject beamMe;
  private string beamMeName;

  public GameObject slotsParent;
  private GameObject[] Slots;


  private float travelLenght; // the distance from a to b
  public bool isMovingOne = false;
  public bool isMoving = false; // used to see if a caught object has reached its destination
  private int numOfTries = 0;


  [Tooltip("When we catched on, should the original keep beeing displayed?")]
  public bool hideWhenCatched;

  [Tooltip("Make sure that each Slot is in this layer, not the childs!!!")]
  public LayerMask SlotLayer;
  private int SlotLayerInt;

  void Start () {

    Slots = new GameObject[slotsParent.transform.childCount];
    for(int i = 0; i < slotsParent.transform.childCount; i++ ) {
      Slots[i] = slotsParent.transform.GetChild(i).gameObject;
    }
    SlotLayerInt = MiscHelper.LayermaskToLayerInt(SlotLayer);

  }


  void FireCatch () {

  }


     /* positions / indexes are shuffeld in SlotsControllerOneObjectAtATime so we need this */
  int getCorrectIndex(int number) {
    var SlotsPositions = SlotsController_A19_multiple.self.SlotIndexes;
    for(int i=0; i<SlotsPositions.Length; i++) {
      if (SlotsPositions[i]==number) {
        return i;
      }
    }
    return -1;
  }



  // Update is called once per frame
  void Update () {

    if (Input.GetMouseButtonUp(0) && MasterActionManager_BS.self.state.batteryLeft >= MasterActionManager_BS.self.state.batteryLoss) {
          numOfTries++;
          // this is alread a Try so fire event
          Messenger<int>.Broadcast("TRIED_CATCH", numOfTries);
        /* wait till a former catch has finished */
        if (!isMovingOne) {
          /* raycast for a slot */
          Vector3 pointto = rayto.transform.position - rayfrom.transform.position;
          RaycastHit2D hit = Physics2D.Raycast(rayto.transform.position, pointto, 100.0f, SlotLayer);
          // Debug.Log(hit);
          if(hit.collider == null) {
            Messenger.Broadcast("CATCH_NOTHING");
            return;
          }
          GameObject currentGo = hit.collider.gameObject;

          GameObject catched = null;

          for (int i = 0; i < currentGo.transform.childCount; i++) {
            GameObject ch = currentGo.transform.GetChild(i).gameObject;
            if(!ch.name.Contains("DONTRENAME")) { // the Hole Hole_DONTRENAME does not count
              catched=ch;
              break;
            }
          }

          if (catched == null) {
            Messenger.Broadcast("CATCH_NOTHING");
            return;
          };
          Messenger<Vector3>.Broadcast("ENEMY_POS", catched.transform.position);

          /* here we proceed and move/catch the object */
          startTime = Time.time;
          beamMeName = catched.name;
          beamMe = Instantiate(catched);
          if (hideWhenCatched) {
            catched.SetActive(false);
          }
          /* default scale ATTENTION, slots have a Scale of 5 so we upscale the childs also */
          beamMe.transform.localScale = catched.transform.localScale * 3 ;
          /* clone the name and keep track of */
          startPos = catched.transform.position;
          travelLenght = Vector3.Distance(startPos, dropZone.transform.position);
          isMovingOne = true;
          /* wait 1 frame with moving, required to show laser also when having a hit */
           return;
        }


    }

    /* one is moving (tracktor beamed ) which works as a kind of delay */
    if (isMovingOne) {
      // do the animation
      isMoving = moveObject(beamMe.transform,startPos,dropZone.transform.position,startTime,travelLenght);
      /* a catched object has been reached the 'destination'
       * reset to default, is not moving one, but caugth one */
      if (!isMoving) {
        isMovingOne = false;
        Messenger<string>.Broadcast("CATCHED_ONE", beamMeName);
        Destroy(beamMe);
      }
    }

  }

   /*
   this method "tractor-beams / move" the object
   */
  bool moveObject(Transform go, Vector3 startMarker, Vector3 endMarker, float startTime, float journeyLenght) {
   // Distance moved = time * speed.
    float distCovered = (Time.time - startTime) * beamSpeed;
    // Fraction of journey completed = current distance divided by total distance.
    float fracJourney = distCovered / travelLenght;
    if (fracJourney > 0.99f) {
      return false;
    }
    // Set our position as a fraction of the distance between the markers.
    go.transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);
    // go.transform.localScale = Vector3.Lerp(go.transform.localScale, new Vector3(.1f,.1f,.1f), fracJourney);
    return true;
  } // end moveObject

} // end class
