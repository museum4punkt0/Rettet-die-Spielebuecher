using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTimeOut : MonoBehaviour
{
    [SerializeField] private float secondsUntilTimeOut = 60;

    private MasterGameManager masterGameManager;
    private float currentTime;

    private bool timerOn = true;

    public GameObject window;
    public TextMeshProUGUI text;

    private Coroutine coroutine;

    void Start()
    {
        masterGameManager = GetComponent<MasterGameManager>();
        masterGameManager.DeleteAllSceneStates();
        currentTime = secondsUntilTimeOut;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            timerOn = !timerOn;
            if (coroutine != null)
                StopCoroutine(coroutine);


            if (timerOn)
            {
                currentTime = secondsUntilTimeOut;
                coroutine = StartCoroutine(ShowToast("on"));
            }
            else
            {
                coroutine = StartCoroutine(ShowToast("off"));
            }
        }

        if (!timerOn)
            return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0.0)
        {
            currentTime = secondsUntilTimeOut;
            masterGameManager.DeleteAllSceneStates();
            SceneManager.LoadSceneAsync("Intro", LoadSceneMode.Single);
        }

        if (Input.anyKey)
        {
            currentTime = secondsUntilTimeOut;
        }
    }

    IEnumerator ShowToast(string newText)
    {
        window.SetActive(true);
        text.text = "timer turned " + newText;
        yield return new WaitForSeconds(1);

        window.SetActive(false);
    }
}