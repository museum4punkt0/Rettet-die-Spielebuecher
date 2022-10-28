
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLoadLevel : MonoBehaviour
{
  Button self;
  
  public string LevelToLoad;

  
    // Start is called before the first frame update
    void Start()
    { 
      self = gameObject.GetComponent<Button>();
      self.onClick.AddListener(LoadLevel);
      
    }

    void LoadLevel() {
      print("load: " + LevelToLoad);
      SceneManager.LoadSceneAsync(LevelToLoad,LoadSceneMode.Single);
    }

   
}
