using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ShowHideNextLevel : MonoBehaviour
{
    public int[] LevelIdsToHide;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        int sceneID = SceneManager.GetActiveScene().buildIndex;
        // Debug.Log("ShowHideNextLevel" + sceneID);
        foreach(int i in LevelIdsToHide) {
          if (i == sceneID) {
            gameObject.SetActive(false);
          }
        }
    }
}
