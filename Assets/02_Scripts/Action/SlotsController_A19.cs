using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
 <summary>

 Let the slots appeard / disapper by scaling them instead of shuffeling
 Make sure to have the same names here and in Quests. Names are used to match!
 This One takes a Generated Object, move this along the slots, when catched die...
 This one distribute ONE Gameobject over the slots!
 Now it is also taking better care of the animations (scale up in slots)
 </summary>
 */
 public class SlotsController_A19 : MonoBehaviour {


 

  /* this data struct is the main game "logic"
   set the Gameobjects to catch here*/
  [System.Serializable]
  public class WhatToCatch {
   public string name;
   public Sprite sprite;
   public GameObject gameObject;
 }

 [Tooltip("Set what to Catch")]
 public WhatToCatch[] whatToCatch;
 [Tooltip("How Many slots should be shown at least")]
 public int[] minimumSlotsToShowUp;

  [Tooltip("You can set this for all Quests (make sure to have length correct). If checked the item to catch always shows up.")]
 public bool[] alwaysShowCatchItem;

 protected GameObject[] Slots;

 [HideInInspector]
 public Vector3[] SlotsPositions;
 [HideInInspector]
 public Vector3[] SlotsScales;
 [HideInInspector]
 public int[] SlotIndexes;
 [HideInInspector]
 public SlotAnimationEventHandlerSPK[] animEvents;


 public bool WorteStattSprites = false;
 // public SpriteRenderer whereToShowQuest;
 // public TextMeshProUGUI WhereShowQuestWord;

 [HideInInspector]
 public int currentQuestID; // the index inside whattoCatch

 private float starttime;
 private IEnumerator IEshuffler; 
 private IEnumerator IEreShow;
 private int[] openSlotIndexes;

 [Tooltip("Time when GO is moved. Can NOT be changed during runtime;")]
 public float[] ShuffleTime;






 protected void Start () {
  /* init the required data to handle logic*/
  int cc = transform.childCount;
  Slots = new GameObject[cc];
  SlotsPositions = new Vector3[cc];
  SlotsScales = new Vector3[cc];
  if (ShuffleTime.Length != whatToCatch.Length) {
     throw new System.NullReferenceException("Set ShuffleTime to the Length of 'What To Catch' !!!");
  }
  animEvents = new SlotAnimationEventHandlerSPK[cc];
  if (minimumSlotsToShowUp.Length != whatToCatch.Length) {
    minimumSlotsToShowUp = new int[whatToCatch.Length];
  }
   if (alwaysShowCatchItem.Length != whatToCatch.Length) {
    alwaysShowCatchItem = new bool[whatToCatch.Length];
  }
  starttime = Time.time;
  SlotIndexes = new int[cc];
  for(int i = 0; i < transform.childCount; i++ ) {
    var slot = transform.GetChild(i).gameObject;
    // SlotAnimationEventHandler handles some parts of animation queing and is required for each slot
    Slots[i] = slot;
    SlotsPositions[i] = slot.transform.position;
    SlotsScales[i] = slot.transform.localScale;
    SlotIndexes[i] = i;
    SlotAnimationEventHandlerSPK animEvent = slot.GetComponent<SlotAnimationEventHandlerSPK>();
    if (null == animEvent) {
         throw new System.NullReferenceException("Each Slots neeeds script SlotAnimationEventHandlerSPK attached");
    }
    animEvents[i] = animEvent;
    if (minimumSlotsToShowUp.Length != whatToCatch.Length) {
      minimumSlotsToShowUp[i] = 2;
    }
    if (alwaysShowCatchItem.Length != whatToCatch.Length) {
      alwaysShowCatchItem[i] = i%2==0 ? true: false;
    }
    
  }

  /* Coroutines */
 
  
  currentQuestID = 0;
  SetcurrentQuestID(currentQuestID);
  // StartCoroutine("ShuffleSlotsPosition",ShuffleTime[currentQuestID]);
}



/* triggered by Message "CATCHED_ONE" */
protected void CatchedOne(string name) {
    /* string replace in name (clone)  since unity does this when instatiate*/
  string cleaned = name.Replace("(Clone)","");
  // Debug.Log("CatchedOne " + cleaned + " " + whatToCatch[currentQuestID].gameObject.name);
  if (cleaned == whatToCatch[currentQuestID].gameObject.name) {
      /*next when available*/
    if (currentQuestID < whatToCatch.Length-1) { currentQuestID++; } 
    else {
      currentQuestID = 0;
      // var completetime = Time.time - starttime;
      // Messenger<float>.Broadcast("QUEST_COMPLETED", completetime);
    }
    /* this is done now by UICatchedUpdate_A19_multiple*/
    // Messenger.Broadcast("CATCH_KORREKT");
  } else {
    Messenger.Broadcast("CATCH_WRONG");

  }
  SetcurrentQuestID(currentQuestID);
  // show next quests
}

/* a helper function to shuffle arrays */
void ShuffleArray<T>(T[] arr) {
 for (int i = arr.Length - 1; i > 0; i--) {
   int r = UnityEngine.Random.Range(0, i);
   T tmp = arr[i];
   arr[i] = arr[r];
   arr[r] = tmp;
 }
}

/* 
 this just moves (shuffles) the slots positions 
 */
private IEnumerator ShuffleSlotsPosition(float waitTime)
{
  // yield return new WaitForSeconds(waitTime);
  while (true)
  {
 
    // shuffle the array with the indexes
    ShuffleArray(SlotIndexes);
    /* howmany with a random length might lead to overlaying slots which cause further errors */
    int howmany = UnityEngine.Random.Range(minimumSlotsToShowUp[currentQuestID],Slots.Length);
   

    /* trigger the closing animation in all slots*/
    for (int i = 0; i < Slots.Length; i++) {
      if (animEvents[i] != null) {
        animEvents[i].closeSlot();
      }
    }

    /* show up to howmany slots: move them to a default defined position and trigger Open Animation */
    for (int i = 0; i<howmany; i++) {
      int ind = SlotIndexes[i];
      /* alwaysShowCatchItem */
      if (alwaysShowCatchItem[currentQuestID]) {
        animEvents[i].showSlot(SlotsPositions[ind]);
      } else {
        animEvents[ind].showSlot(SlotsPositions[i]);
      }
    }
    Messenger.Broadcast("SHUFFLE_SLOTS");
    yield return new WaitForSeconds(waitTime);
  }
}



/* 
 * Setup a new Item to catch, i.e. place it as a child of slot 1 
 */
void SetcurrentQuestID(int index) 
{ 
  /* there might be older hidden elements inside the slots
  so destroy everything except the "HOLES "*/
  for (int i = 0; i < Slots.Length; i++) {
    GameObject s = Slots[i];
    for (int j = 0; j < s.transform.childCount; j++) {
      GameObject ch = s.transform.GetChild(j).gameObject;
      if(!ch.name.Contains("DONTRENAME")) { 
        // Debug.Log("Destroy " + ch.name + " " + j);
        Destroy(ch);
      }
    }
  }
    /* instantiate the Gameobject from whatToCatch based on index  */
    GameObject theQuested = Instantiate(
      whatToCatch[index].gameObject,
      Slots[0].transform.position , // its always in the first slot, slot position change
      whatToCatch[index].gameObject.transform.rotation,
      Slots[0].transform // set slot 0 as parent
      );
    theQuested.SetActive(true); // might be hidden by Catch Script

    /* show the user some info what to catch */
    // if (WorteStattSprites == false) {
    //   whereToShowQuest.sprite = whatToCatch[index].sprite;
    //   WhereShowQuestWord.enabled = false;
    // } else {
    //   whereToShowQuest.enabled = false;
    //   WhereShowQuestWord.text = whatToCatch[index].name;
    // }

    /* start the animation */
    // if (IEshuffler != null) {
    //   StopCoroutine(IEshuffler);
    // }
    // StartCoroutine(IEshuffler);
    StopCoroutine("ShuffleSlotsPosition");
    StartCoroutine("ShuffleSlotsPosition",ShuffleTime[currentQuestID]);
  }



/* triggered by Message "QUEST_COMPLETE" */

 protected void QuestComplete (float timeStamp) {
    // PauseToggle(true);
    /* move self out of screen */
    // transform.position = new Vector3(100f,100f,100f);
  }


  protected void PauseToggle(bool pause) {
    // Debug.Log("GAME_PAUSED GAME_PAUSED" + pause);
    // if (pause) {StopCoroutine(animateQuestSlots); }
    // else { StartCoroutine(animateQuestSlots); }
  }







}
