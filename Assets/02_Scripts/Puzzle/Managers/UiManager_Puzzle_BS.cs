using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* this is a rather thumb script listening to events only */
public class UiManager_Puzzle_BS : MonoBehaviour
{
    public GameObject StartupPlate;
    public GameObject SolvedPlate;
    public GameObject FirstTimeVisitInstructions;
    // Start is called before the first frame update
    // void Start()
    // {
    //   if (StartupPlate != null && showStartupPlate) {
    //     StartupPlate.SetActive(true);
    //     showStartupPlate = false;
    //   }
    // }
    //

    [Range(0.0f,4.0f)]
    public float waitAfterSolve;

    public AudioClip clipSuccess;
    [Range(0f,1f)]
    public float successVolume;
    private AudioSource audioSuccess;


    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol, float pitch) {
      AudioSource newAudio = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
      newAudio.clip = clip;
      newAudio.loop = loop;
      newAudio.playOnAwake = playAwake;
      newAudio.volume = vol;
      newAudio.pitch = pitch;
      return newAudio;
    }

    void Awake() {
        Messenger.AddListener("FIRST_TIME_IN_PUZZLE", firstTimeVisitPuzzle);
        Messenger.AddListener("PUZZLE_SOLVED", puzzleSolved);
        audioSuccess = AddAudio(clipSuccess, false, false, successVolume, 1.0f);
     }
    void OnDestroy(){
        Messenger.RemoveListener("FIRST_TIME_IN_PUZZLE", firstTimeVisitPuzzle);
        Messenger.RemoveListener("PUZZLE_SOLVED", puzzleSolved);
     }


     void puzzleSolved(){
          StartCoroutine("DelayedDisplay_SolvedPlate",waitAfterSolve);
     }

     private IEnumerator DelayedDisplay_SolvedPlate(float time) {
        yield return new WaitForSeconds(time);
        audioSuccess.Play();
        SolvedPlate.SetActive(true);
     }

     void firstTimeVisitPuzzle() {
          Debug.Log("FIRST_TIME_IN_PUZZLE");
          StartCoroutine("DelayedDisplay_FirstTimeVisitInstructions", 1f);
     }

      private IEnumerator DelayedDisplay_FirstTimeVisitInstructions(float time) {
        yield return new WaitForSeconds(time);
        FirstTimeVisitInstructions.SetActive(true);
     }

}
