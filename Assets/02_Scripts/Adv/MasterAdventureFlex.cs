using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 
-contains the logic between the objects (player, enemies, collectibles, book) in the scene 
-fires events for the UI 
-manages help system via events and state
- needs to run a init script (since player might get back to here) and set the state off
    -- enemies
    -- book
    -- collectibles (might change)
- therefor it needs to connect to master game (state) manager
*/
public class MasterAdventureFlex : MonoBehaviour
{
    public static MasterAdventureFlex self;

    /* book state */
    // [HideInInspector]
    // public bool book_isopen;
   
    // [HideInInspector]
    // [Tooltip("No Functionality at the moment")]
    // public bool book_showsbubble = false;

    public bool hideWimmeltiere;
    bool hideWimmeltiereInternal;
    public bool hideCollectibles = true;
    bool hideCollectiblesInternal = false;

    public GameObject WimmeltierParent;

    private GameObject collectiblesParent;

   

    [HideInInspector]
    List<int> collectedIds = new List<int>();
    List<string> collectedNames = new List<string>();

    public string NameOfPuzzle;
    public string NameOfAction;
    
    public GameObject book;
    public GameObject book_bubble;
    // public Sprite book_bubble_sprite;
    public GameObject book_required_go; // what we need to collect before book opens

    private float startTime;
    private float timeSpend;

    private GameObject player;

    Puzzle_State puzzle_state; // this is the state of Puzzle
    SerialState_Action action_state; // action
    Adventure_State state; // this is the state of Adventure

    void parse_Action_State() {
        action_state = new SerialState_Action();
        string status = "";
        if (MasterGameManager.master != null) {
            status = MasterGameManager.master.getSceneState(NameOfAction);
            if (status != "") {
                action_state = JsonUtility.FromJson<SerialState_Action>(status);
                Debug.Log(status);
            }
        } 
    }

    /* state of puzzle */
    void parse_Puzzle_State() {
        puzzle_state = new Puzzle_State();  
        string status = "";
        if (MasterGameManager.master != null) {
            status = MasterGameManager.master.getSceneState(NameOfPuzzle);
            if (status != "") {
                Debug.Log(status);
                puzzle_state = JsonUtility.FromJson<Puzzle_State>(status);
            }
        } 
        // Debug.Log(puzzle_state.is_solved);

        // else {
        //      /* MOCK DATA FOR DEV */
        //     Puzzle_State mockData = new Puzzle_State();
        //     mockData.is_solved = false; // show success plate 
        //     mockData.is_solvable = false;  // not in use 
        //     mockData.visited = false; // when false show "collect more " or/and help
        //     mockData.timeSpend = 0f; // some stats
        //     puzzle_state = mockData;
        // }
    }

    void retrieveState() {
        state = new Adventure_State();  
        string status = "";
        if (MasterGameManager.master != null) {
            Scene scene = SceneManager.GetActiveScene();
            status = MasterGameManager.master.getSceneState(scene.name);
        } 
        if (status != "") {
            state = JsonUtility.FromJson<Adventure_State>(status);
            Debug.Log(status);
        } else {
             /* MOCK AND DEFAULT DATA  */
            // Adventure_State mockdata = new Adventure_State(); 
            // mockdata.collectedIds = new List<int>();  // this refers to the Index (Order) of the items to collect, can not serialize gameobjects
            // mockdata.collectedNames = new List<string>(); // required in puzzle, better controll 
            // // mockdata.collectedNames.Add("CollectMe");
            // mockdata.playerPosition = player.transform.position;
            // state = mockdata;
        }
    }
    
    /*  state should be reported to the Master_Level_Manager */
    void writeState() {
        state.collectedIds = collectedIds;
        state.collectedNames = collectedNames;
        state.timeSpend = timeSpend +  Time.time - startTime;
        state.playerPosition = player.transform.position;
         string status = JsonUtility.ToJson(state);
        // Debug.Log(status);
        if (MasterGameManager.master != null) {
            Scene scene = SceneManager.GetActiveScene();
            MasterGameManager.master.saveSceneState(scene.name,status);
        }
    }



    void Start() {
    // void Awake() {
        startTime = Time.time;
        parse_Puzzle_State();
        parse_Action_State();
        self = this;
        collectiblesParent = GameObject.Find("Collectibles_DONOTRENAME");
        player = GameObject.Find("Player");
        if (null == player || null == collectiblesParent) {
           throw new System.NullReferenceException("Make sure to have 'Collectibles_DONOTRENAME' and 'Player' GameObjects");
        }
        retrieveState();

        if (state.playerPosition.y != 0f) {
            player.transform.position = state.playerPosition;
        }
        StartCoroutine("keepTrackOfState",0.5f);

        /* hide all collectible when puzzle has not been visited*/
        if (puzzle_state.visited == false) {
            HideCollectibles(true);
        } else {
            // HideCollectibles(false);
        }
        /* hide the ones already collected */        
        foreach(int i in collectedIds) {
            collectiblesParent.transform.GetChild(i).gameObject.SetActive(false);
        }

        // if (hideWimmeltiere) {
        //     HideWimmeltiere(true);
        // }
        // if(hideCollectibles) {
        //     HideCollectibles(true);
        // }

        if(action_state.is_solved) {
            HideCollectibles(true);
            // HideWimmeltiere(true);
        }
        /* save the current State to master */
      
    }



    void HideWimmeltiere (bool onOff) {
        for (int i = 0; i<WimmeltierParent.transform.childCount; i++) {
            WimmeltierParent.transform.GetChild(i).gameObject.SetActive(!onOff);
        }
        hideWimmeltiereInternal = hideWimmeltiere;
    }

     void HideCollectibles (bool onOff) {
        // Debug.Log("HideCollectibles" + onOff);
        for (int i = 0; i<collectiblesParent.transform.childCount; i++) {
            collectiblesParent.transform.GetChild(i).gameObject.SetActive(!onOff);
        }
        hideCollectiblesInternal = hideCollectibles;
    }

            // Update is called once per frame
    void Update()
    {
        // if (hideWimmeltiereInternal != hideWimmeltiere) {
        //     HideWimmeltiere(hideWimmeltiere);
        // }
        // if (hideCollectiblesInternal != hideCollectibles) {
        //     HideCollectibles(hideCollectibles);
        // }
        // book_bubble.SetActive(book_showsbubble);
        // book_required_go.SetActive(false);
        // if (collectedIds.Contains(book_required_go)) {
        //     book.GetComponent<Animator>().SetBool("open", true);
        // } 
    }


    /* bump into book */
    public void characterHitBook() {
        book.GetComponent<Animator>().SetBool("open", true);
        if (puzzle_state.is_solved) {
            StartCoroutine("DelayedLoadActionLevel",1f);
            /* load action level */
        } else if (!puzzle_state.visited) {
            StartCoroutine("DelayedBookPlateDisplay",1f);
        } else {
             StartCoroutine("DelayedLoadPuzzleLevel",1f);
        }
        // Messenger.Broadcast("BUMP_INTO_BOOK");
        // Debug.Log("characterHitBook");
        /* player collected the item, open book */
        // if (collectedIds.Contains(book_required_go)) {
        //     // Debug.Log("collected.Contains(book_required_go)");
        //     // Messenger<float>.Broadcast("QUEST_COMPLETED",0f);
        // } else {
        //     book_showsbubble = true;
        //     book_bubble.SetActive(true);
        //     book_required_go.SetActive(true);
        //     StartCoroutine("TalkBubbleOff",2f);
        // }
    }
     private IEnumerator DelayedLoadPuzzleLevel(float time) {
      yield return new WaitForSeconds(time);
      SceneManager.LoadSceneAsync(NameOfPuzzle,LoadSceneMode.Single);
  }
    private IEnumerator DelayedLoadActionLevel(float time) {
      yield return new WaitForSeconds(time);
      SceneManager.LoadSceneAsync(NameOfAction,LoadSceneMode.Single);
  }

    public void characterHitExitarea(){
          Messenger.Broadcast("BUMP_INTO_EXITAREA");
    }


    public void characterHitEnemy(GameObject enemy) {
          if(action_state.is_solved) {
            Messenger.Broadcast("BUMP_INTO_WIMMEL_SOLVED");
            StartCoroutine("DelayedLoadWimmeltierHide", 1f);
          } else {
            Messenger.Broadcast("BUMP_INTO_WIMMEL");
            var enemyA = enemy.GetComponent<Animator>();
            if (enemyA != null) {
                enemyA.SetBool("OnceHit", true);
            }
          }
        // Debug.Log("characterHitEnemy" + enemyA);
    }

private IEnumerator DelayedLoadWimmeltierHide(float time) {
      yield return new WaitForSeconds(time);
        HideWimmeltiere(true);
      
  }
    
    /* we need to save the index/order inside of Collectibles_DONOTRENAME
     found no other way to serialize this
     */   
    private int getIndexOfCollectable (GameObject which) {
        for (int i = 0; i<collectiblesParent.transform.childCount; i++) {
            if (collectiblesParent.transform.GetChild(i).gameObject == which) {
                return i;
            }
        }
        return -1;
    }
    
    public void characterCollectItem(GameObject collect) {
        int index = getIndexOfCollectable(collect);
        if (index != -1) {
            collectedIds.Add(index);
            collectedNames.Add(collect.name);
            collect.SetActive(false);
            Messenger<int>.Broadcast("COLLECTED_ONE", index);
        }
    }

    
   private IEnumerator keepTrackOfState(float time) {
        while(true) {
            writeState();
            yield return new WaitForSeconds(time);
        }
    }


  private IEnumerator DelayedBookPlateDisplay(float time) {
      yield return new WaitForSeconds(time);
      // book_bubble.SetActive(false);
      Messenger.Broadcast("BUMP_INTO_BOOK");
  }


  

  private IEnumerator TalkBubbleOff(float time) {
      yield return new WaitForSeconds(time);
      book_bubble.SetActive(false);
  }



}
 