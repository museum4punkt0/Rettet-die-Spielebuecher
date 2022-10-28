using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadSceneWithoutButton : MonoBehaviour
{
   
    public string LevelToLoad;


    // Update is called once per frame
    void Update()
    {
       if (gameObject.activeSelf && Input.GetMouseButtonUp(0))  {
              SceneManager.LoadSceneAsync(LevelToLoad,LoadSceneMode.Single);
       }

    }
}
