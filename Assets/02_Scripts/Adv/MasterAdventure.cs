using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
public class MasterAdventure : MonoBehaviour
{
    public static MasterAdventure self;

    /* book state */
    [HideInInspector]
    public bool book_isopen;
    [HideInInspector]
    public bool book_wasopened;
    [HideInInspector]
    public bool book_showsbubble;

    // [HideInInspector]
    public List<GameObject> collected = new List<GameObject>();
    
    public GameObject book;
    public GameObject book_bubble;
    public Sprite book_bubble_sprite;
    public GameObject book_required_go;


    public void characterHitBook() {
        // Debug.Log("characterHitBook");
        /* player collected the item, open book*/
        if (collected.Contains(book_required_go)) {
            // Debug.Log("collected.Contains(book_required_go)");
            book.GetComponent<Animator>().SetBool("open", true);
            Messenger<float>.Broadcast("QUEST_COMPLETED",0f);
        } else {
            book_bubble.SetActive(true);
            book_required_go.SetActive(true);
            StartCoroutine("TalkBubbleOff",2f);
        }
    }


    public void characterHitEnemy(GameObject enemy) {
        var enemyA = enemy.GetComponent<Animator>();
        Debug.Log("characterHitEnemy" + enemyA);
        if (enemyA != null) {
            enemyA.SetBool("OnceHit", true);
        }
        
    }
   
    
    public void characterCollectItem(GameObject collect) {
        // Debug.Log("Character Collect " + collect.name);
        collected.Add(collect);
        collect.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        self = this;
        book_bubble.SetActive(false);
        book_required_go.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        manageBookState();
    }

    void manageBookState(){
      if (book_showsbubble) {
        book_bubble.SetActive(true);
      }

    }

       /* this one requires that we have less object than slots*/
  private IEnumerator TalkBubbleOff(float time) {
      yield return new WaitForSeconds(time);
      book_bubble.SetActive(false);
  }



}
