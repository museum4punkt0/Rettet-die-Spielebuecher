using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/*
-contains the logic between the objects (player, enemies, collectibles, book) in the scene
-fires events for the UI
-manages help system via events and state
- needs to run a init script (since player might get back to here) and set the state of
    -- enemies
    -- book
    -- collectibles (might change)
- therefor it needs to connect to master game (state) manager
*/
[RequireComponent(typeof(PlayerCharacterSelected))]
public class MasterAdventureFlex_BS : MonoBehaviour
{
    [Tooltip("When checked this levels shows all the help/instructional plates")]
    public bool showFirstTimeInstructions = false;

    public static MasterAdventureFlex_BS self;

    /* book state */
    // [HideInInspector]
    // public bool book_isopen;

    // [HideInInspector]
    // [Tooltip("No Functionality at the moment")]
    // public bool book_showsbubble = false;

    // public GameObject[] characters;
    private int character = 0;

    public bool hideWimmeltiere;
    bool hideWimmeltiereInternal;
    public bool hideCollectibles = true;
    bool hideCollectiblesInternal = false;

    public GameObject WimmeltierParent;

    public GameObject batteriesParent;

    private GameObject collectiblesParent;


    [HideInInspector]
    List<int> collectedIds = new List<int>();
    [SerializeField]
    List<string> collectedNames = new List<string>();

    public string NameOfPuzzle;
    public string NameOfAction;

    public GameObject book;
    public GameObject book_bubble;
    // public Sprite book_bubble_sprite;
    // public GameObject book_required_go; // what we need to collect before book opens

    private float startTime;
    private float timeSpend;

    private GameObject player;

    Charakter_State charakter_state;
    Puzzle_State puzzle_state; // this is the state of Puzzle
    Action_State action_state; // action
    Adventure_State state; // this is the state of Adventure

    /* battery stuff */
    public RectTransform BatteryWhite;
    public RectTransform BatteryPercentage;
    private float batteryMaxHeight;
    private float batteryWidth;
    private float batteryLeft;
    private Vector3 initBatteryScale;
    // [Range(0.0f, 1.0f)]
    // public float BatteryUpAmt;

    /**/
    [Tooltip("This is required to keep track of FOG in between levels")]
    public FogOfWarScript fogOfWarScript;



    /*  state is reported to the Master_Level_Manager,
    method is called in a fixed intervall */
    void writeState() {
        state.collectedIds = collectedIds;
        state.collectedNames = collectedNames;
        state.timeSpend = timeSpend +  Time.time - startTime;
        state.playerPosition = player.transform.position;
        state.batteryLeft = batteryLeft;
        if (fogOfWarScript != null && fogOfWarScript.fogAlphas != null && fogOfWarScript.fogAlphas.Length > 0 ) {
            // Debug.Log("ADV status " +   JsonHelper.ToJson(state.fogAlphas));
           state.fogAlphas = fogOfWarScript.fogAlphas;
          
        }
        string status = JsonUtility.ToJson(state);
        if (MasterGameManager.master != null) {
            Scene scene = SceneManager.GetActiveScene();
            MasterGameManager.master.saveSceneState(scene.name,status);
        }
    }

/*
* When adv gets load it needs info from the state of puzzle and action part
* here we have a lot of fine logic, e.g. show or hide batteries based on game state
*/

    void Start() {

    // void Awake() {
        startTime = Time.time;
        puzzle_state = GameStates.Get_Puzzle_State(NameOfPuzzle);
        action_state = GameStates.Get_Action_State(NameOfAction);
        charakter_state = GameStates.Get_Charakter_State();

        self = this;
        collectiblesParent = GameObject.Find("Collectibles_DONOTRENAME");
        player = GameObject.Find("Player");

        if (null == player || null == collectiblesParent) {
           throw new System.NullReferenceException("Make sure to have 'Collectibles_DONOTRENAME' and 'Player' GameObjects");
        }

        /* charakter things */
        PlayerCharacterSelected pcs = GetComponent<PlayerCharacterSelected>();
        if (null == pcs) {
           throw new System.NullReferenceException("Make sure to have 'PlayerCharacterSelected' on GO");
        }
        if(charakter_state.characterID != null) {
          character = charakter_state.characterID;
        } else {
          character = 0;
        }
        pcs.characters[character].SetActive(true);
        pcs.characters[character].transform.position = player.transform.position;

        /* 
        * handle self state 
        */
        Scene scene = SceneManager.GetActiveScene();
        state = GameStates.Get_Adventure_State(scene.name);

        /*
         NOW Logic based on game state 
        */
        if(action_state.visited == false){
            // full batterie on new level
            batteryLeft = 1.0f; 
        }

        /* setting position to where it was when left level*/
        if (state.playerPosition.y != 0f) {
            player.transform.position = state.playerPosition;
        }

        // Debug.Log(JsonUtility.ToJson(state));
        // Debug.Log(JsonUtility.ToJson(puzzle_state));
        // Debug.Log(JsonUtility.ToJson(action_state));

        

        if (puzzle_state.visited == false && showFirstTimeInstructions) {
            Messenger.Broadcast("SHOW_STARTUP_PLATE");
            // state.visited = true;
        }

        if (puzzle_state.visited == false || puzzle_state.is_solved == true) {
            HideCollectibles(true);
        } else {
            HideCollectibles(false); // show them we user was in book
            
            /* hide the ones already collected */
            collectedIds = state.collectedIds;
            collectedNames = state.collectedNames;
            foreach(var name in collectedNames)
            {
                GameObject.Find(name).SetActive(false);
            }
        }

        // first hide all Batteries
        HideBatteries(true);
        // show if not full
        batteryLeft = action_state.batteryLeft;
        
        print("-------battery: " + batteryLeft + " / isDone: " + action_state.is_done + " ------------");
        if(batteryLeft < 1.0f && !action_state.is_done) {
            print("-----inside------");
            HideBatteries(false);
        } 
        
        if(action_state.is_solved) {
            HideCollectibles(true);
            HideWimmeltiere(true);

            if (action_state.is_done)
            {
                HideBatteries(true);
            }
        }
        
        state.visited = true;
        /* save the current State to master */
        StartCoroutine("keepTrackOfState",0.5f);

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

    void HideBatteries (bool off) {
      // when switch off, always hide them all
      if (off) {
          for (int i = 0; i<batteriesParent.transform.childCount; i++) {
            batteriesParent.transform.GetChild(i).gameObject.SetActive(false);
          }
      } else {
         int showOnlyOne = Random.Range(0,batteriesParent.transform.childCount); // max is exclusive
         batteriesParent.transform.GetChild(showOnlyOne).gameObject.SetActive(true);
      }
    }

            // Update is called once per frame
    void Update()
    {
      // Resize battery pecentage to match remaining battery
      // BatteryPercentage.localScale = initBatteryScale + new Vector3(0f, batteryLeft*batteryMaxHeight, 0f);
    }



    public void characterHitBook() {
        if (action_state.is_done)
        {
            return;
        }
        
        book.GetComponent<Animator>().SetBool("open", true);
        if (puzzle_state.is_solved && !action_state.is_done) {
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
            // collectedNames.Add(collect.name);
            // collect.SetActive(false);
            // Messenger<int>.Broadcast("COLLECTED_ONE", 0);
            int index = getIndexOfCollectable(collect);
            if (index != -1) {
                collectedIds.Add(index);
                collectedNames.Add(collect.name);
                collect.SetActive(false);
                Messenger<int>.Broadcast("COLLECTED_ONE", index);
            }
        // Debug.Log("Character Collect " + getIndexOfCollectable(collect));
    }

    public void characterCollectBattery(GameObject collect) {
        // if(batteryLeft + BatteryUpAmt >= 1.0f)
        //   batteryLeft = 1.0f;
        // else
        batteryLeft = 1.0f;//batteryLeft + BatteryUpAmt;
        collect.SetActive(false);
        // if(BatteryPercentage.gameObject.GetComponent<Image>().color == Color.red){
        //   BatteryPercentage.gameObject.GetComponent<Image>().color = Color.HSVToRGB(0.34167f, 0.7f, 0.8f);
        //   StopCoroutine("BlinkRed");
        // }
        // if(BatteryPercentage.gameObject.activeSelf != true)
        //   BatteryPercentage.gameObject.SetActive(true);
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

  IEnumerator BlinkRed(float time) {
    while (true)
    {
      yield return new WaitForSeconds(time);
      BatteryPercentage.gameObject.SetActive(!BatteryPercentage.gameObject.activeSelf);
    }
  }

  void SetUpBatteryUI(){
    // Define battery percentage meter dimensions
    batteryMaxHeight = (BatteryWhite.rect.height * BatteryWhite.localScale.y)*0.675f;
    batteryWidth = (BatteryWhite.rect.width * BatteryWhite.localScale.x) - 90.0f;

    // Place battery percentage meter on battery UI
    BatteryPercentage.SetParent(BatteryWhite.parent, false);
    BatteryPercentage.rotation = BatteryWhite.rotation;
    BatteryPercentage.localPosition = BatteryWhite.localPosition - new Vector3(0f,batteryMaxHeight*0.525f,0f);
    BatteryPercentage.localScale += new Vector3(batteryWidth,0f,0f);
    initBatteryScale = BatteryPercentage.localScale;
    BatteryPercentage.gameObject.SetActive(true);
  }


}
