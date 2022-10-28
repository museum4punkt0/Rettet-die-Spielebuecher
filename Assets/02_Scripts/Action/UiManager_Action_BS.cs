using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager_Action_BS : MonoBehaviour
{

  public bool showSuccessAfterEach = true;
  public bool showBadCatchAfterEach = true;
  public GameObject successParent;
  public GameObject BadCatchParent;
  public GameObject QuestCompleted;
  public GameObject StartupPlate;
  public GameObject OutOfBatteryPlate;


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

  [Tooltip("Set the borders for Stars Ranking"),
  Range(1,30)]
  public int ZeroStarsUpperBorder = 25;
  [Tooltip("Set the borders for Stars Ranking"),
  Range(30,50)]
  public int OneStarsUpperBorder = 45;
  [Tooltip("Set the borders for Stars Ranking"),
  Range(40,70)]
  public int TwoStarsUpperBorder = 70;


  public Sprite RatingStarActive;

  /* Displaying Stats*/
  // public GameObject DisplayTime;
  // public GameObject DisplayTries;
  // public GameObject DisplayFalse;
  // public GameObject DisplayShuffle;

  private int shuffleSuccessPlates = 0;
  private int shuffleBadCatchPlates = 0;


  /* battery stuff */
  public RectTransform BatteryWhite;
  public RectTransform BatteryPercentage;
  private float batteryMaxHeight;
  private float batteryWidth;
  private float batteryLeft;
  private Vector3 initBatteryScale;
  private float batteryLoss = 0.0f;


  [Tooltip("The (child) gameObject of with a script of type UICatchedUpdate_A19_multiple (the one which takes care of the UI (showing which item to catch)")]
  public GameObject UIGameLogikGO;
  private int GamePlayRounds;


  void Awake() {
   
    Messenger.AddListener("SHOW_STARTUP_PLATE", ShowStartupPlate);
    // Messenger<bool>.AddListener("GAME_PAUSED", PauseToggle);
    Messenger<float>.AddListener("QUEST_COMPLETED", QuestComplete);

    Messenger<int>.AddListener("TRIED_CATCH", CountTriedCatch);
    Messenger.AddListener("CATCH_KORREKT", CatchedCorrect);
    Messenger.AddListener("CATCH_WRONG", CatchWrong);
    Messenger.AddListener("CATCH_NOTHING", CatchNothing);
    Messenger.AddListener("SHUFFLE_SLOTS", CountShuffleSlot);
    Messenger.AddListener("HIDE_STARTUP_PLATE", HideStartupPlate);
    // Messenger.AddListener("SHOW_STARTUP_PLATE", ShowStartupPlate);
    Messenger.AddListener("OUT_OF_BATTERY", ShowOutOfBatteryPlate);
    StartCoroutine("delayedGetInitBattery",0.01f);
  }


  void Start() {

    /* how many "rounds"*/
     UICatchedUpdate_A19_multiple UIGameLogikScript = UIGameLogikGO.GetComponent<UICatchedUpdate_A19_multiple>();
    if (null == UIGameLogikScript) {
      throw new System.NullReferenceException("We need script of type UICatchedUpdate_A19_multiple");
    }
    GamePlayRounds = 0;
    for (int i=0; i< UIGameLogikScript.whatToCatchUI.Length; i++) {
      GamePlayRounds += UIGameLogikScript.whatToCatchUI[i].count;
    }

    /* hide these plate */
    int count = successParent.transform.childCount;
    for(int i=0; i<count; i++) {
      Transform child = successParent.transform.GetChild(i);
      child.gameObject.SetActive(false);
    }
    /* hide these plate */
    count = BadCatchParent.transform.childCount;
    for(int i=0; i<count; i++) {
      Transform child = BadCatchParent.transform.GetChild(i);
      child.gameObject.SetActive(false);
    }
    
    // if (StartupPlate != null) {
    //   StartupPlate.SetActive(true);
    // }
    // QuestCompleted.SetActive(false);
    numOfShuffles=0;
    numOfTries = 0;
    starttime = Time.time;
    successCount = 0;

    // Define battery percentage meter dimensions
    batteryMaxHeight = (BatteryWhite.rect.height * BatteryWhite.localScale.y)*0.675f;
    batteryWidth = (BatteryWhite.rect.width * BatteryWhite.localScale.x) - 90.0f;

    // Place battery percentage meter on battery UI
    BatteryPercentage.SetParent(BatteryWhite.parent, false);
    BatteryPercentage.rotation = BatteryWhite.rotation;
    BatteryPercentage.localPosition = BatteryWhite.localPosition - new Vector3(0f,batteryMaxHeight*0.525f,0f);
    BatteryPercentage.localScale += new Vector3(batteryWidth,0f,0f);
    initBatteryScale = BatteryPercentage.localScale;
    BatteryPercentage.gameObject.SetActive(true);


  }
  void OnDestroy(){
   Messenger<float>.RemoveListener("QUEST_COMPLETED", QuestComplete);
   Messenger<int>.RemoveListener("TRIED_CATCH", CountTriedCatch);
   // Messenger.RemoveListener("SHOW_STARTUP_PLATE", showStartUpPlate);
   Messenger.RemoveListener("CATCH_KORREKT", CatchedCorrect);
   Messenger.RemoveListener("CATCH_WRONG", CatchWrong);
   Messenger.RemoveListener("CATCH_NOTHING", CatchNothing);
   Messenger.RemoveListener("SHUFFLE_SLOTS", CountShuffleSlot);
   Messenger.RemoveListener("HIDE_STARTUP_PLATE", HideStartupPlate);
   Messenger.RemoveListener("SHOW_STARTUP_PLATE", ShowStartupPlate);
   Messenger.RemoveListener("OUT_OF_BATTERY", ShowOutOfBatteryPlate);


 }




  void Update(){
    // Resize battery pecentage to match remaining battery
    BatteryPercentage.localScale = initBatteryScale + new Vector3(0f, batteryLeft*batteryMaxHeight, 0f);
  }

  void ShowStartupPlate() {
    // Debug.Log("ShowStartupPlate");
    StartupPlate.SetActive(true);
  }
  void HideStartupPlate() {
    StartupPlate.SetActive(false);
  }

  void ShowOutOfBatteryPlate(){
    OutOfBatteryPlate.SetActive(true);
  }


  void reduceBattery() {
    // Subtract BatteryLoss from battery percentage
    batteryLoss = MasterActionManager_BS.self.state.batteryLoss;
      /* for demo reasons, we LFK added or TRUE */
    if(batteryLeft >= batteryLoss || true ){
      if(batteryLeft < 2*batteryLoss){
        BatteryPercentage.gameObject.GetComponent<Image>().color = Color.red;
        StartCoroutine("BlinkRed",0.5f);
        Messenger.Broadcast("OUT_OF_BATTERY");
      }
      batteryLeft -= batteryLoss;
    }
    else{
      batteryLeft = 0;
      Messenger.Broadcast("OUT_OF_BATTERY");
    }
    MasterActionManager_BS.self.state.batteryLeft = batteryLeft;
  }

  void CatchWrong() {
    Debug.Log("CatchWrong");
    showNextWrongCatchPlate(0);
    reduceBattery();
  }
  
  void CatchNothing() {
    Debug.Log("Catch Nothing here");
    showNextWrongCatchPlate(2);
    reduceBattery();
  }

  void CountTriedCatch(int num) {
    reduceBattery();
    numOfTries++;
    // Debug.Log(numOfTries + " numOfTries");
  }

void CountShuffleSlot(){
    numOfShuffles++;
  // Debug.Log(numOfShuffles + "numOfShuffles");
}
void showNextWrongCatchPlate(int index){
  Debug.Log("showNextWrongCatchPlate " + index);
  if(showBadCatchAfterEach) {
    Transform child = BadCatchParent.transform.GetChild(index);
    child.gameObject.SetActive(true);
  }
}

void showNextSuccessCatchPlate(){
  if(showSuccessAfterEach) {
     Transform child = successParent.transform.GetChild(shuffleSuccessPlates);
     child.gameObject.SetActive(true);
  }
  shuffleSuccessPlates++;
  if(shuffleSuccessPlates >= successParent.transform.childCount) {
    shuffleSuccessPlates = 0;
  }
}

 void CatchedCorrect() {
    showNextSuccessCatchPlate();
    successCount++;
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


  /* 
   * here we calculate the stats to a 3 star rating:
   * taking into account: 
   *  rounds (num of quests in total)
   *  
   *  from:
   *  0-30% : 0 stars
   *  30-50% : 1 star
   *  50-80% : 2 stars
   *  80%-100%: 3 stars
   */

void QuestComplete(float completeTime) {

  /* how many "rounds"*/
  
  var state = GameStates.Get_Action_State(SceneManager.GetActiveScene().name);

  print("gamePLayRounds: " + GamePlayRounds);
  print("numOfTries: " + numOfTries);
  print("state.batteriesPickedUp: " + state.batteriesPickedUp);
  if (GamePlayRounds >= numOfTries - 2 && state.batteriesPickedUp == 1)
  {
    Transform rt3 = QuestCompleted.transform.Find("RatingStar_3");
    rt3.gameObject.GetComponent<Image>().sprite = RatingStarActive;
    
    Transform rt2 = QuestCompleted.transform.Find("RatingStar_2");
    rt2.gameObject.GetComponent<Image>().sprite = RatingStarActive;
    
    Transform rt1 = QuestCompleted.transform.Find("RatingStar_1");
    rt1.gameObject.GetComponent<Image>().sprite = RatingStarActive;
  } 
  else if (state.batteriesPickedUp <= 2)
  {
    Transform rt1 = QuestCompleted.transform.Find("RatingStar_1");
    rt1.gameObject.GetComponent<Image>().sprite = RatingStarActive;

    Transform rt2 = QuestCompleted.transform.Find("RatingStar_2");
    rt2.gameObject.GetComponent<Image>().sprite = RatingStarActive;
  }
  else
  {
    Transform rt1 = QuestCompleted.transform.Find("RatingStar_1");
    rt1.gameObject.GetComponent<Image>().sprite = RatingStarActive;
  }
  
 //else 
 //{
 //  float percentageRounds = (float)GamePlayRounds / (float)numOfShuffles;
 //  float percentageTrys = (float)GamePlayRounds / (float)numOfTries;
 //  float overallPerfomance = (percentageRounds + percentageTrys) / 2;
 //  Debug.Log("percentageRounds " + percentageRounds);
 //  Debug.Log("percentageTrys " + percentageTrys);
 //  Debug.Log("overallPerfomance " + overallPerfomance);

 //  if (overallPerfomance * 100 >= ZeroStarsUpperBorder)
 //  {
 //    Transform rt1 = QuestCompleted.transform.Find("RatingStar_1");
 //    Image ici = rt1.gameObject.GetComponent<Image>();
 //    ici.sprite = RatingStarActive;
 //  }

 //  if (overallPerfomance * 100 >= OneStarsUpperBorder)
 //  {
 //    Transform rt1 = QuestCompleted.transform.Find("RatingStar_2");
 //    Image ici = rt1.gameObject.GetComponent<Image>();
 //    ici.sprite = RatingStarActive;
 //  }

 //  if (overallPerfomance * 100 >= TwoStarsUpperBorder)
 //  {
 //    Transform rt1 = QuestCompleted.transform.Find("RatingStar_3");
 //    Image ici = rt1.gameObject.GetComponent<Image>();
 //    ici.sprite = RatingStarActive;
 //  }
  //}

  // float timeNeed = Time.time - starttime;
  // if( null != DisplayTime) {
  //     var me = DisplayTime.GetComponent<Text>();
  //     me.text = "Zeit: " + timeNeed;
  // }
  // if( null != DisplayTries) {
  //    var me = DisplayTries.GetComponent<Text>();
  //    me.text = "Anzahl Versuche: " + numOfTries;
  // }
  // if( null != DisplayFalse) {
  //    var me = DisplayFalse.GetComponent<Text>();
  //    me.text = "Fehlversuche: " + (numOfTries - successCount);
  // }
  // if( null != DisplayShuffle) {
  //    var me = DisplayShuffle.GetComponent<Text>();
  //    me.text = "Anzahl Spielrunden: " + numOfShuffles;
  // }

  Messenger<string>.Broadcast("NEW_ACTION_DATA",SerializedActionData());
  QuestCompleted.SetActive(true);
}

private IEnumerator delayedGetInitBattery (float time) {
    yield return new WaitForSeconds(time);
    batteryLeft = MasterActionManager_BS.self.state.batteryLeft;
    if(batteryLeft < 2*MasterActionManager_BS.self.state.batteryLoss){
      BatteryPercentage.gameObject.GetComponent<Image>().color = Color.red;
      StartCoroutine("BlinkRed",0.5f);
    }
}

IEnumerator BlinkRed(float time) {
  while (true)
  {
    yield return new WaitForSeconds(time);
    BatteryPercentage.gameObject.SetActive(!BatteryPercentage.gameObject.activeSelf);
    // blink
  }
}

}
