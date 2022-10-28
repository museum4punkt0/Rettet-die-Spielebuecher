using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event_State_Action : MonoBehaviour
{

  public bool showSuccessAfterEach = true;
  public GameObject successParent;
  public GameObject QuestCompleted;
  public GameObject StartupPlate;


  /* counting and stats*/
  private float starttime;
  private int numOfTries;
  private int numOfShuffles;
  private int successCount;

  [System.Serializable]
  public class SerialActionData {
    public float timeNeed;
    public int numOfTries;
    public int numOfShuffles;
    public int successCount;
  }

  /* Displaying Stats*/
  public GameObject DisplayTime;
  public GameObject DisplayTries;
  public GameObject DisplayFalse;
  public GameObject DisplayShuffle;

  private Vector2[] SuccessdefaultSizes;
  private int shuffleSuccessPlates = 0;

  void Start() {
    int count = successParent.transform.childCount;
    SuccessdefaultSizes = new Vector2[count];
    for(int i=0; i<count; i++) {
      Transform child = successParent.transform.GetChild(i);
      // RectTransform rt = child.gameObject.GetComponent<RectTransform>();
      // SuccessdefaultSizes[i] = rt.sizeDelta;
      child.gameObject.SetActive(false);
    }
    if (StartupPlate != null) {
      StartupPlate.SetActive(true);
    }
    // QuestCompleted.SetActive(false);
    numOfShuffles=0;
    numOfTries = 0;
    starttime = Time.time;
    successCount = 0;
  }
  void OnDestroy(){
   Messenger<float>.RemoveListener("QUEST_COMPLETED", QuestComplete);
   Messenger<int>.RemoveListener("TRIED_CATCH", CountTriedCatch);
   Messenger.RemoveListener("CATCH_KORREKT", CatchedCorrect);
   Messenger.RemoveListener("CATCH_WRONG", CatchWrong);
   Messenger.RemoveListener("SHUFFLE_SLOTS", CountShuffleSlot);
 }

  void Awake() {
    // Messenger<bool>.AddListener("GAME_PAUSED", PauseToggle);
    Messenger<float>.AddListener("QUEST_COMPLETED", QuestComplete);
    Messenger<int>.AddListener("TRIED_CATCH", CountTriedCatch);
    Messenger.AddListener("CATCH_KORREKT", CatchedCorrect);
    Messenger.AddListener("CATCH_WRONG", CatchWrong);
    Messenger.AddListener("SHUFFLE_SLOTS", CountShuffleSlot);
    
  }

  void CatchWrong() {
    /* not yet*/
  }

  void CountTriedCatch(int num) {
    numOfTries++;
    // Debug.Log(numOfTries + " numOfTries");
  }

void CountShuffleSlot(){
    numOfShuffles++;
  // Debug.Log(numOfShuffles + "numOfShuffles");
}
 


 void CatchedCorrect() {

    if(showSuccessAfterEach) {
     Transform child = successParent.transform.GetChild(shuffleSuccessPlates);
     // Debug.Log("CatchedCorrect " + child.name);
     child.gameObject.SetActive(true);
    }
   successCount++;
   shuffleSuccessPlates++;
   if(shuffleSuccessPlates >= successParent.transform.childCount) {
    shuffleSuccessPlates = 0;
   }
 }

public string SerializedActionData() {
  float timeNeed = Time.time - starttime;
  SerialActionData data = new SerialActionData();
    data.timeNeed = timeNeed;
    data.numOfTries = numOfTries;
    data.numOfShuffles = numOfShuffles;
    data.successCount = successCount;
  return JsonUtility.ToJson(data);
}



void QuestComplete(float completeTime) {
  // Debug.Log("QuestComplete");
  float timeNeed = Time.time - starttime;
  if( null != DisplayTime) {
      var me = DisplayTime.GetComponent<Text>();
      me.text = "Zeit: " + timeNeed;
  }
  if( null != DisplayTries) {
     var me = DisplayTries.GetComponent<Text>();
     // Debug.Log(numOfTries + " numOfTries");
     me.text = "Anzahl Versuche: " + numOfTries;
  }
  if( null != DisplayFalse) {
     var me = DisplayFalse.GetComponent<Text>();
     me.text = "Fehlversuche: " + (numOfTries - successCount); 
  }
  if( null != DisplayShuffle) {
     var me = DisplayShuffle.GetComponent<Text>();
     me.text = "Anzahl Spielrunden: " + numOfShuffles;
  }
  Messenger<string>.Broadcast("NEW_ACTION_DATA",SerializedActionData());
  QuestCompleted.SetActive(true);
}
 

}
