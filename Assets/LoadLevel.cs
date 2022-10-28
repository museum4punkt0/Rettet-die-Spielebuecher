using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] private String LevelToLoad;

    private void OnEnable()
    {
        SceneManager.LoadSceneAsync(LevelToLoad, LoadSceneMode.Single);
        gameObject.SetActive(false);
    }
}