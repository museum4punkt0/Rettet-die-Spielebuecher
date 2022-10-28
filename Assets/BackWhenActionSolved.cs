using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackWhenActionSolved : MonoBehaviour
{
    [SerializeField] private string levelToLoad;
    [SerializeField] private string actionLevelToCheckFrom;

    [SerializeField] private GameObject hideObject;

    private bool solved = false;

    private void Start()
    {
        var state = GameStates.Get_Action_State(actionLevelToCheckFrom);
        solved = state.is_solved;

        if (solved)
        {
            hideObject?.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (solved)
            {
                SceneManager.LoadScene(levelToLoad, LoadSceneMode.Single);
            }
        }
    }
}