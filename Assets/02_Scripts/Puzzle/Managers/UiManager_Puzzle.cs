using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* this is a rather thumb script listening to events only */
public class UiManager_Puzzle : MonoBehaviour
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
    [Range(0.0f,4.0f)]
    public float waitBeforeShowStartup = 3.0f;
    public bool showSolvedPlateOnlyOnce = true;
    public bool showButtonWhenSolved = true;
    [Tooltip("A Button to go to action level, which will be shown when puzzle is solved and showButtonWhenSolved is true ")]
    public GameObject NextButton;
    private bool solvedPlateShown = false;

    void Awake() {
        Messenger.AddListener("FIRST_TIME_IN_PUZZLE", firstTimeVisitPuzzle);
        Messenger.AddListener("SHOW_STARTUP_PLATE", showStartUpPlate);
        Messenger.AddListener("PUZZLE_SOLVED", puzzleSolved);
        Messenger.AddListener("SHOW_NEXT", showNext);
    }
    void OnDestroy(){
        Messenger.RemoveListener("FIRST_TIME_IN_PUZZLE", firstTimeVisitPuzzle);
        Messenger.RemoveListener("SHOW_STARTUP_PLATE", showStartUpPlate);
        Messenger.RemoveListener("PUZZLE_SOLVED", puzzleSolved);
        Messenger.RemoveListener("SHOW_NEXT", showNext);
     }


   void puzzleSolved(){
      if (!solvedPlateShown || !showSolvedPlateOnlyOnce) {
        solvedPlateShown = true;
        StartCoroutine("DelayedDisplay_SolvedPlate",waitAfterSolve);
      }
      if(showButtonWhenSolved && NextButton!= null ) {
        NextButton.SetActive(true);
      }
      // Debug.Log("PUZZLE SOLVED");
   }

    void showStartUpPlate() {
      // StartupPlate.SetActive(true);
       StartCoroutine("DelayedDisplay_NeedMore_Collectibles",waitBeforeShowStartup);
    }

  void showNext(){
      if(showButtonWhenSolved && NextButton!= null ) {
        NextButton.SetActive(true);
      }
        // Debug.Log("SHOW NEXT BUTTON");
   }

private IEnumerator DelayedDisplay_NeedMore_Collectibles(float time) {
      yield return new WaitForSeconds(time);
      while (!Input.GetMouseButtonDown(0)) {
        yield return null;
      }
      
      StartupPlate.SetActive(true);
  }

private IEnumerator DelayedDisplay_SolvedPlate(float time) {
      yield return new WaitForSeconds(time);
      SolvedPlate.SetActive(true);
  }

   void firstTimeVisitPuzzle() {
        // Debug.Log("FIRST_TIME_IN_PUZZLE");
        StartCoroutine("DelayedDisplay_FirstTimeVisitInstructions", 1f);
   }

    private IEnumerator DelayedDisplay_FirstTimeVisitInstructions(float time) {
      yield return new WaitForSeconds(time);
      FirstTimeVisitInstructions.SetActive(true);
  }

}
