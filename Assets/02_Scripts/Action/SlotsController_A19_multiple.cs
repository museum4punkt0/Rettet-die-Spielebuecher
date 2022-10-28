using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* extends the SlotsController_A19 behaviour
adds other (wrong) objects
 */
public class SlotsController_A19_multiple : SlotsController_A19
{
  // public bool showOther = true;
  // public int howManyOther = 1;

   public static SlotsController_A19_multiple self;
  
 void Awake() {
    self = this; // have it static
    Messenger<bool>.AddListener("GAME_PAUSED", PauseToggle);
    Messenger<float>.AddListener("QUEST_COMPLETED", QuestComplete);
    Messenger<string>.AddListener("CATCHED_ONE", CatchedOne);
  }

 void OnDestroy(){
   Messenger<bool>.RemoveListener("GAME_PAUSED", PauseToggle);
   Messenger<float>.RemoveListener("QUEST_COMPLETED", QuestComplete);
   Messenger<string>.RemoveListener("CATCHED_ONE", CatchedOne);
 }


  void Start() {
    base.Start();
    StartCoroutine("PopulateGlutterObjects",ShuffleTime[currentQuestID]);
    // StartCoroutine("PopulateGlutterObjects",0);
  }

  /* triggered by Message "CATCHED_ONE" */
  void CatchedOne(string name) {
    // Debug.Log("CatchedOne " + name);
    base.CatchedOne(name);
    StopCoroutine("PopulateGlutterObjects");
    StartCoroutine("PopulateGlutterObjects",ShuffleTime[currentQuestID]);
    // StartCoroutine("PopulateGlutterObjects",0);
  }

  /*
  
  */
  private IEnumerator PopulateGlutterObjects(float waitTime) {
    // Debug.Log("PopulateGlutterObjects");
    // for whatToCatch;
    for(int i = 0; i < whatToCatch.Length; i++ ) {
      if (i != currentQuestID) {
        if (i+1 < Slots.Length) {
          // Debug.Log("PopulateGlutterObjects " + Slots[i+1].transform.childCount);
          // Slots[i+1] 
           /* instantiate the Gameobject from whatToCatch based on index  */
          GameObject Clutter = Instantiate(
            whatToCatch[i].gameObject,
            Slots[i+1].transform.position , // its always in the first slot, slot position change
            whatToCatch[i].gameObject.transform.rotation,
            Slots[i+1].transform // set slot 0 as parent
            );
          Clutter.SetActive(true); // might be hidden by Catch Script
        }
      }
    }

    yield return new WaitForSeconds(waitTime);
  }
}
