using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* this is the overall master script managing "STATE" in between the different levels  */
public class LevelManager_Action : MonoBehaviour {
  
  public string[] sceneNames;
  [SerializeField]
  private Dictionary<string,string> sceneStates;

  [SerializeField]
  private int currentScene = 0;

  public GameObject UIText_time; 
  public GameObject UIText_numOfTries; 
  public GameObject UIText_level; 
  public Button Next_Button; 
  public Button Pause_Button; 

  bool paused = false;
  /* tracking time*/
  float starttime;

  void Awake(){
    DontDestroyOnLoad(gameObject);
    /* fill the dict with states */
    foreach (string sceneName in sceneNames) {
      sceneStates.Add(sceneName, "");
    }
    Messenger<float>.AddListener("QUEST_COMPLETED", QuestCompleted);
    Messenger<int>.AddListener("TRIED_CATCH", CountTries);
  }


  /* */
  public void saveSceneState(string sceneName, string state) {
    sceneStates[sceneName] = state;
  }

    /* */
  public string getSceneState(string sceneName) {
      string value;
      if (sceneStates.TryGetValue(sceneName, out value)) {
        return value;
      } else {
        return "";
      }
  }

  void OnDestroy(){
    Messenger<float>.RemoveListener("QUEST_COMPLETED", QuestCompleted);
    Messenger<int>.RemoveListener("TRIED_CATCH", CountTries);
  }

  void Start(){
    Pause_Button.onClick.AddListener(OnClickPause);
    Next_Button.onClick.AddListener(OnClickNext);
    // Next_Button.gameObject.SetActive(false);
    // Application.LoadLevel(sceneNames[currentScene]);
    // SceneManager.LoadScene(sceneNames[currentScene],LoadSceneMode.Additive);
    // SceneManager.LoadScene(sceneNames[currentScene]);
    // SceneManager.LoadSceneAsync(sceneNames[currentScene]);
     // Use a coroutine to load the Scene in the background
    StartCoroutine(LoadYourAsyncScene(sceneNames[currentScene]));
    // Scene scene = SceneManager.GetActiveScene();
    // Debug.Log("Active scene is '" + scene.name + "'.");
    starttime = Time.time;
    Text t = UIText_level.GetComponent<Text>();
    var readlevel = currentScene+1;
    t.text = "Level " + readlevel;
  }

  void QuestCompleted (float time) {
    // Debug.Log("QuestCompleted " + time);
    // Messenger<bool>.Broadcast("GAME_PAUSED", true);
    var completetime = Time.time - starttime;
    Next_Button.gameObject.SetActive(true);
    Text t = UIText_time.GetComponent<Text>();
    var readlevel = currentScene+1;
    t.text = "Level" + readlevel  + " in " + completetime + "s";
  }

  void CountTries (int numOfTries) {
    var t = UIText_numOfTries.GetComponent<Text>();
    t.text = "Versuche: " + numOfTries;
  }

    void OnClickPause(){
      // Debug.Log("OnClickPause" + paused);
      paused = !paused;
      Messenger<bool>.Broadcast("GAME_PAUSED", paused);
    }

    void OnClickNext(){
      currentScene++;

      // Debug.Log("OnClickNext" + currentScene);
      if (currentScene==sceneNames.Length) { // all quests completed
        currentScene=0;
      } 

        // Application.LoadLevel(sceneNames[currentScene]);
         // SceneManager.LoadScene(sceneNames[currentScene],LoadSceneMode.Additive);
         SceneManager.LoadScene(sceneNames[currentScene]);

        // Next_Button.gameObject.SetActive(false);
        paused = false;
      // Messenger<bool>.Broadcast("GAME_PAUSED", paused);
      starttime = Time.time;
      Text t = UIText_level.GetComponent<Text>();
      t.text = "Level " + currentScene;
      
    }

    /*
     *https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html
     */
    IEnumerator LoadYourAsyncScene(string name)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

      

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

  


}
