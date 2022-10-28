using UnityEngine;
// using UnityEngine.UI;
using System.Collections;

/* 
This script is attached to the UI GO in scene
for demo reasons, load 2 simple scenes by UI-Button Click
and have UI itself be persistent 

*/

public class UIManagerPersistent : MonoBehaviour { 
  

  

  void Awake(){
    DontDestroyOnLoad(gameObject);
    // Messenger<float>.AddListener("QUEST_COMPLETED", QuestCompleted);
  }

  void OnDestroy(){
    // Messenger<float>.RemoveListener("QUEST_COMPLETED", QuestCompleted);
  }
  


  void QuestCompleted (float time) {
    // Debug.Log("QuestCompleted " + time);
  }

  public void OnButtonLoadScene(string name) {
    // Application.LoadLevel(name);
  }


  private void OnGameComplete() {
    // levelEnding.gameObject.SetActive(true);
    // levelEnding.text = "You Finished the Game!";
  }

  public void SaveGame() {
    // Managers.Data.SaveGameState();
  }

  public void LoadGame() {
    // Managers.Data.LoadGameState();
  }
}
