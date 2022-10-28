using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* this is the overall master script managing "STATE" in between the different levels  */
public class MasterGameManager : MonoBehaviour {
  
  public static MasterGameManager master;

  public string[] sceneNames;
  [SerializeField]
  private Dictionary<string,string> sceneStates;

  [SerializeField]
  private int currentScene = 0;

  public bool ShowNextButtonAlways = true;


  public Button Next_Button; 
  public Button Pause_Button; 

  bool paused = false;
  /* tracking time*/
  float starttime;

  void Awake(){
    DontDestroyOnLoad(gameObject);
    master = this;
    Messenger<float>.AddListener("QUEST_COMPLETED", QuestCompleted);
  }

  void OnDestroy(){
    Messenger<float>.RemoveListener("QUEST_COMPLETED", QuestCompleted);
  }

  

  /* */
  public void saveSceneState(string sceneName, string state) {
    // Debug.Log("saveSceneState called "  + sceneName + ": " + state );
    sceneStates[sceneName] = state;
  }

       /* */
  public string getSceneState(string sceneName) {
      string value;
      if (sceneStates.TryGetValue(sceneName, out value)) {
        // Debug.Log("GET STATE" + sceneName + ": " + value );
        return value;
      } else {
        return "";
      }
  }



  void Start(){
    sceneStates = new Dictionary<string,string>();
     /* fill the dict with states */
    foreach (string sceneName in sceneNames) {
      sceneStates.Add(sceneName, "");
    }

    Pause_Button.onClick.AddListener(OnClickPause);
    Next_Button.onClick.AddListener(OnClickNext);
     // Use a coroutine to load the Scene in the background
    StartCoroutine(LoadYourAsyncScene(sceneNames[currentScene]));
    // Scene scene = SceneManager.GetActiveScene();
    // Debug.Log("Active scene is '" + scene.name + "'.");
    starttime = Time.time;
    
  }

  void QuestCompleted (float time) {
    // Debug.Log("QuestCompleted " + time);
    // Messenger<bool>.Broadcast("GAME_PAUSED", true);
    var completetime = Time.time - starttime;
    if(ShowNextButtonAlways) {
      Next_Button.gameObject.SetActive(true);
    }
   
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
         SceneManager.LoadSceneAsync(sceneNames[currentScene],LoadSceneMode.Single);
         // SceneManager.LoadScene(sceneNames[currentScene]);

        // Next_Button.gameObject.SetActive(false);
        paused = false;
      // Messenger<bool>.Broadcast("GAME_PAUSED", paused);
      starttime = Time.time;

      
    }

    public void DeleteAllSceneStates()
    {
      sceneStates = new Dictionary<string, string>();
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

        // AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
         AsyncOperation asyncLoad =  SceneManager.LoadSceneAsync(sceneNames[currentScene],LoadSceneMode.Single);

      

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

  


}
