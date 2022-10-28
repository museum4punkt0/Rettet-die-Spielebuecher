using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;





[RequireComponent(typeof(PlayerCharacterSelected))]
public class MasterPuzzleManager : MonoBehaviour
{
    public string nameOfAdventure; // for going back button
    public string nameOfAtionLevel; // for going forward to action part
    // [Tooltip("Set the name of the GameObjects (from Adventure) required to solve the puzzle")]
    // public string[] requiredItems;

    [Tooltip("Gamelogic here, taking collectedNames from Adventure. For each of these we show an item (draggable puzzle piece)")]
    public MapNamesToDraggables[] mapDraggables;

    /* this is used to set up level logik*/
    [System.Serializable]
     public class MapNamesToDraggables
     {
        [Tooltip("Set here, whats required to solve the puzzle. The Names (NameFromAdv) must match the elements collected/collectable in adventure")]
        public string NameFromAdv;
        public GameObject Draggable;
        [Tooltip("when user already placed this item / solved it, then position it here")]
        public Vector3 solvedPosition; 
        public bool isSolved;
     }

    /* 
    * SELF STATE : INTERNAL RUNTIME STATE 
    */
    [SerializeField]
    bool showStartupPlate = true;  // 
    bool is_solved;
    bool visited = true;
    [SerializeField]
    public List<string> puzzleParts = new List<string>();
    [SerializeField]
    public List<bool> puzzlePartsSolved = new List<bool>();
    float timeSpend;
    float startTime;

    Charakter_State charakter_state;
    Puzzle_State state; // this is the state of the PUZZLE / self
    Adventure_State adv_state; // this is the state of Adventure



   



    /* write state to master */
    void writeState(){
        state.is_solved = is_solved;
        state.visited = visited;
        state.timeSpend = Time.time - startTime; // this is more for data 
        state.puzzleParts = new List<string>(puzzleParts);;
        state.puzzlePartsSolved = new List<bool>(puzzlePartsSolved);;
        string status = JsonUtility.ToJson(state);
        if (MasterGameManager.master != null) {
            Scene scene = SceneManager.GetActiveScene();
            MasterGameManager.master.saveSceneState(scene.name,status);
        }
    }

     void Awake() {
        // Messenger.AddListener("PUZZLE_SOLVED", puzzleSolved);
        Messenger<string>.AddListener("PIN_AT_PLACE", pinAtPlace);
        Messenger<string>.AddListener("PIN_AWAY_PLACE", pinAwayFromPlace);
    }
    void OnDestroy(){
        // Messenger.RemoveListener("PUZZLE_SOLVED", puzzleSolved);
        Messenger<string>.RemoveListener("PIN_AT_PLACE", pinAtPlace);
        Messenger<string>.RemoveListener("PIN_AWAY_PLACE", pinAwayFromPlace);
     }

    void Start() {
        startTime = Time.time;
        Scene scene = SceneManager.GetActiveScene();
        charakter_state = GameStates.Get_Charakter_State();
        state = GameStates.Get_Puzzle_State(scene.name);
        adv_state = GameStates.Get_Adventure_State(nameOfAdventure);


        puzzleParts = new List<string>(state.puzzleParts);
        puzzlePartsSolved = new List<bool>(state.puzzlePartsSolved);
         /* 
         in case we have no state (related to puzzle parts) 
         we are initialising these lists based on mapDraggables
         */
         if (puzzleParts.Count == 0) {
            foreach(MapNamesToDraggables it in mapDraggables){
                // Debug.Log(it.NameFromAdv);
                puzzleParts.Add(it.NameFromAdv);
                puzzlePartsSolved.Add(false);
            }
         }

        PlayerCharacterSelected pcs = GetComponent<PlayerCharacterSelected>();
        if(charakter_state.characterID != null) {
          // character = PlayerSettings.Robot;
            pcs.characters[charakter_state.characterID].SetActive(true);
         
        } else {
            pcs.characters[0].SetActive(true);
          // character = 0;
        }
       
        
        /* walk through MapNamesToDraggables[] mapDraggables
          * NameFromAdv;
            Draggable;
         */
        bool foundAll = true;
        bool foundNone = true;
        foreach(MapNamesToDraggables it in mapDraggables){
            Debug.Log(adv_state.collectedNames.Contains(it.NameFromAdv) + " " + it.NameFromAdv);
            if (adv_state.collectedNames.Contains(it.NameFromAdv)) {
                // foundNone = false;
                foundNone = false;
                it.Draggable.SetActive(true);
                /* check to see if it is in "Solved Puzzle Parts" */
             /* 
                we have this but then we need to set manually all solved positions
                so for now, we keep this feature off:
                features was: once user placed an item at the right place, its is solved the next time she visits the puzzle
              */
                // for (int i = 0; i < puzzleParts.Count; i++) {
                //     if (puzzleParts[i] == it.NameFromAdv && puzzlePartsSolved[i] == true) {
                //         // it.Draggable.transform.localPosition = it.solvedPosition;
                //         // it.isSolved = true;
                //         // it.isSolved = false; // we require all to be collected 
                //     }
                // }
            } else {
                foundAll = false;
                it.Draggable.SetActive(false);
            }
        }
      
        writeState();
        StartCoroutine("CheckForSolved",1f);
        Debug.Log(foundAll);
        if(foundAll == false) {
            // Debug.Log("We need this important SHOW_STARTUP_PLATE");
            
            // this contains the button "Back to Adventure", means puzzle is not solvable
            
            Messenger.Broadcast("SHOW_STARTUP_PLATE"); 
        } else {
            // this contains the instructions "Ziehe das Teil"
            Messenger.Broadcast("FIRST_TIME_IN_PUZZLE"); 
        }
        visited = true;
      
       



        
    }

    void pinAtPlace( string nameofPin) {
        Debug.Log("pinAtPlace " + nameofPin);
        for (int i = 0; i < mapDraggables.Length; i++) {
            if (mapDraggables[i].Draggable.name==nameofPin) {
                puzzlePartsSolved[i] = true;
            }
        }
        writeState();
    }

    void pinAwayFromPlace( string nameofPin) {
        for (int i = 0; i < mapDraggables.Length; i++) {
            if (mapDraggables[i].Draggable.name==nameofPin) {
                puzzlePartsSolved[i] = false;
            }
        }
          writeState();
    }

    /*this is obsolete, we check state now for "solved"*/
     void puzzleSolved(){
        is_solved = true;
        writeState();
     }



    private IEnumerator CheckForSolved(float time) {
        while (true) {
            // Debug.Log("CheckForSolved " + puzzlePartsSolved.Count);
            bool allSolved = false;
            for (int i = 0; i < puzzlePartsSolved.Count; i++) {
                // Debug.Log(puzzlePartsSolved[i]);
               if (puzzlePartsSolved[i]==true) {
                allSolved = true;
               } else {
                allSolved = false;
                break;
                }
            }
            is_solved = allSolved;
            
            var state = GameStates.Get_Action_State(nameOfAtionLevel);

            if (is_solved && !state.is_solved) {
                string status = JsonUtility.ToJson(state);

                print("-------next level----" + status);
                // Debug.Log("PUZZLE IS SOLVED");
                Messenger.Broadcast("SHOW_NEXT");
                Messenger.Broadcast("PUZZLE_SOLVED");
            }
            writeState();
            yield return new WaitForSeconds(time);
        }
    }


}
