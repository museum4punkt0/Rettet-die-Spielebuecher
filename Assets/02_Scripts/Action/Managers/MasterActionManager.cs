using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* 
 * this is the state used to 'talk' to other levels, we keep track of these 
 * listing to "outer" events and dispatched "inner events"
 */

[System.Serializable]
public class SerialState_Action {
    public bool is_solved = false; // show success plate 
    public bool visited = false; // when false show "collect more " or/and help
    public bool showStartUpPlate = false;
    // public GameObject[] puzzleParts; // we do not track them for now
    // public Transform[] puzzlePartsPositions; // we do not track them for now
    public float timeSpend = 0f; // some stats
}

public class MasterActionManager : MonoBehaviour
{
    Adventure_State adv_state; // this is the state of Adventure
    SerialState_Action state; // this is the state

    bool is_solved;
    bool visited;
    bool showStartUpPlate;
    float startTime;

    public GameObject ResetButton;
    public string nameOfAdventure; // for going back button
    // public string nameOfAtionLevel; // for going forward to action part

    void retrieveState() {
        Scene scene = SceneManager.GetActiveScene();
        state = new SerialState_Action();  
        string status = "";
        if (MasterGameManager.master != null) {
            status = MasterGameManager.master.getSceneState(scene.name);
            // Debug.Log(status);
        } 
        // do we have/get a status
        if (status != "") {
            state = JsonUtility.FromJson<SerialState_Action>(status);
        } 
         else {
             /* MOCK DATA FOR DEV */
            SerialState_Action mockData = new SerialState_Action();
            mockData.is_solved = false;
            mockData.visited = false;
            mockData.showStartUpPlate = true;
            state = mockData;
        }
    }
        /* write state to master */
    void writeState(){
        state.is_solved = is_solved;
        state.visited = visited;
        state.showStartUpPlate = showStartUpPlate;
        state.timeSpend = Time.time - startTime; // this is more for data 
        // Debug.Log(status);
        /* mock data*/
        if (MasterGameManager.master != null) {
            string status = JsonUtility.ToJson(state);
            Scene scene = SceneManager.GetActiveScene();
            MasterGameManager.master.saveSceneState(scene.name,status);
        }
    }

  void OnDestroy(){
   Messenger<float>.RemoveListener("QUEST_COMPLETED", QuestComplete);
  }

  void Awake() {
    // Messenger<bool>.AddListener("GAME_PAUSED", PauseToggle);
    Messenger<float>.AddListener("QUEST_COMPLETED", QuestComplete);
  }

  void QuestComplete(float time) {
    is_solved = true;
  }

  void init() {
        startTime = Time.time;
        retrieveState();
        showStartUpPlate = state.showStartUpPlate;
        if(is_solved) {
          showStartUpPlate = false;
        }
        // Debug.Log("showStartUpPlate" + showStartUpPlate);
        // Debug.Log("is_solved" + is_solved);
        if(showStartUpPlate) {
          Messenger.Broadcast("SHOW_STARTUP_PLATE");
        } else {
          Messenger.Broadcast("HIDE_STARTUP_PLATE");
        }
        StartCoroutine("keepTrackOfState",0.5f);
        if (ResetButton != null) {
          Button ResetButtonButton = ResetButton.GetComponent<Button>();
          ResetButtonButton.onClick.AddListener(ResetMe);
        }
  }

  void ResetMe(){
    showStartUpPlate = false;
    writeState();
    StartCoroutine("delayedReload",0.5f);
  }


    void Start() 
    {
      init();
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
