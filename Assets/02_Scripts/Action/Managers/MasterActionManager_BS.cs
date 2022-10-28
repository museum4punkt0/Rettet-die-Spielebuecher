using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * this is the state used to 'talk' to other levels, we keep track of these
 * listing to "outer" events and dispatched "inner events"
 */


public class MasterActionManager_BS : MonoBehaviour
{
    public static MasterActionManager_BS self;
    [HideInInspector]
    public Adventure_State adv_state; // this is the state of Adventure
    [HideInInspector]
    public Action_State state; // this is the state

   

    bool is_solved;
    bool visited;
    bool showStartUpPlate;
    float startTime;
    private bool isDone;
    
    public GameObject ResetButton;
    public string nameOfAdventure; // for going back button

    [Range(0.0f, 1.0f)]
    public float BatteryLoss;
    public bool LoseBatteryOnSuccessfulShot;
    
    
    private Coroutine keepTrackRoutine;

    private int batteriesPickedUp = 0;
    
    /* report state to master */
    void writeState(){
      var state = GameStates.Get_Action_State(SceneManager.GetActiveScene().name);

        state.is_solved = is_solved;
        state.is_done = isDone;
        state.visited = visited;
        state.showStartUpPlate = showStartUpPlate;
        state.timeSpend = Time.time - startTime; // this is more for data
        state.batteryLoss = BatteryLoss;
        state.batteryLeft = 0;
        state.whatToCatchUI = UICatchedUpdate_A19_multiple.singleton.whatToCatchUI;
        state.batteriesPickedUp = batteriesPickedUp;

        if (MasterGameManager.master != null) {
            string status = JsonUtility.ToJson(state);
            Scene scene = SceneManager.GetActiveScene();
            MasterGameManager.master.saveSceneState(scene.name,status);
        }
    }

  void OnDestroy(){
   Messenger<float>.RemoveListener("QUEST_COMPLETED", QuestComplete);
   writeState();
  }

  void Awake() {
    // Messenger<bool>.AddListener("GAME_PAUSED", PauseToggle);
    Messenger<float>.AddListener("QUEST_COMPLETED", QuestComplete);
    visited = true;
  }

  void QuestComplete(float time) {
    is_solved = true;
    isDone = true;
  }


  void Start() {
        self = this;
        startTime = Time.time;
        Scene scene = SceneManager.GetActiveScene();
        state = GameStates.Get_Action_State(scene.name);
        adv_state = GameStates.Get_Adventure_State(nameOfAdventure);

        batteriesPickedUp = state.batteriesPickedUp + 1;

        // if(adv_state.batteryLeft <= 1.0f){
        // } else{
        //   state.batteryLeft  = 1.0f;
        // }
        state.batteryLeft = adv_state.batteryLeft;

        is_solved = state.is_solved;
        state.batteryLoss = BatteryLoss;
        showStartUpPlate = state.showStartUpPlate;
        if(is_solved) {
          showStartUpPlate = false;
        }
        // Debug.Log("showStartUpPlate" + showStartUpPlate);
        // Debug.Log("is_solved" + is_solved);
        if(showStartUpPlate) {
          Messenger.Broadcast("SHOW_STARTUP_PLATE");
          state.showStartUpPlate = false;
        } else {
          Messenger.Broadcast("HIDE_STARTUP_PLATE");
        }
        
        if (state.whatToCatchUI != null && 
            state.whatToCatchUI.Length > 0 &&
            (state.whatToCatchUI[0].count > 0 || state.whatToCatchUI[1].count > 0))
        {
          UICatchedUpdate_A19_multiple.singleton.whatToCatchUI = state.whatToCatchUI;
        }
        
       keepTrackRoutine = StartCoroutine("keepTrackOfState",0.5f);
        if (ResetButton != null) {
          Button ResetButtonButton = ResetButton.GetComponent<Button>();
          ResetButtonButton.onClick.AddListener(ResetMe);
        }
  }

  void ResetMe(){
    showStartUpPlate = false;
    
    StopCoroutine(keepTrackRoutine);

    isDone = false;
    
    state = GameStates.Get_Action_State(SceneManager.GetActiveScene().name);
    batteriesPickedUp = 0;
    state.is_solved = is_solved;
    state.is_done = false;
    state.visited = visited;
    state.showStartUpPlate = showStartUpPlate;
    state.timeSpend = Time.time; // this is more for data
    state.batteryLoss = BatteryLoss;
    state.batteryLeft = 0;
    state.batteriesPickedUp = 0;
    state.whatToCatchUI = UICatchedUpdate_A19_multiple.singleton.baseWhatToCatchUI;
        
    if (MasterGameManager.master != null) {
      string status = JsonUtility.ToJson(state);
      Scene scene = SceneManager.GetActiveScene();
      MasterGameManager.master.saveSceneState(scene.name,status);
    }
    
    StartCoroutine("delayedReload",0.5f);
  }

  private IEnumerator delayedReload (float time) {
        yield return new WaitForSeconds(time);
        Scene scene = SceneManager.GetActiveScene();
          SceneManager.LoadSceneAsync(scene.name,LoadSceneMode.Single);
    }

     private IEnumerator keepTrackOfState(float time) {
        while(true) {
            writeState();
            yield return new WaitForSeconds(time);
        }
    }



}
