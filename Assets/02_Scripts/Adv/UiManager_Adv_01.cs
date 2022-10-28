using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/* 
 * listen to events and manages UI
 * 
 *  -- enemies
    -- book
    -- collectibles (might change)
 */
  /* each child in Collectibles_DONOTRENAME need an other child called Sprite with a spriterender */
public class UiManager_Adv_01 : MonoBehaviour
{
  public static UiManager_Adv_01 self;

    /* collectibles */
  public GameObject DisplayCollectibles;
  public Material InactiveCollectiblesMat;
  public Material ActiveCollectiblesMat;
  public GameObject collectiblesParent;

  public GameObject BumpIntoWimmeltierPlate;
  public GameObject BumpIntoWimmeltierPlateSolved;
  public GameObject BumpIntoBook;
  public GameObject ExitAreaPlate;

  RectTransform DisplayCollectiblesRectTransform;  // accessing rectrans
  Image[] collectibleRep;

  void Start() {
    self = this;
    SetUpUIRepOfCollectibles();
  }

  void Awake() {
    Messenger<int>.AddListener("COLLECTED_ONE", collectedOne);
    Messenger.AddListener("BUMP_INTO_WIMMEL", bumpIntoWimmel);
    Messenger.AddListener("BUMP_INTO_WIMMEL_SOLVED",bumpIntoWimmelSolved);
    Messenger.AddListener("BUMP_INTO_BOOK", bumpIntoBook);
    Messenger.AddListener("BUMP_INTO_EXITAREA", bumpIntoExitArea);
  }
  void OnDestroy(){
    Messenger<int>.RemoveListener("COLLECTED_ONE", collectedOne);
    Messenger.RemoveListener("BUMP_INTO_WIMMEL",bumpIntoWimmel);
    Messenger.RemoveListener("BUMP_INTO_WIMMEL_SOLVED",bumpIntoWimmelSolved);
    Messenger.RemoveListener("BUMP_INTO_BOOK", bumpIntoBook);
    Messenger.RemoveListener("BUMP_INTO_EXITAREA", bumpIntoExitArea);
 }

    /* set the collected in UI to Active = Collected*/
  public void bumpIntoBook() {
   BumpIntoBook.SetActive(true);
  }

  public void bumpIntoWimmelSolved() {
   BumpIntoWimmeltierPlateSolved.SetActive(true);
  }


  public void bumpIntoWimmel() {
    // BumpIntoWimmeltierPlate_Solved
   BumpIntoWimmeltierPlate.SetActive(true);
  }

  public void bumpIntoExitArea() {
    // Debug.Log("BUMP_INTO_EXITAREA");
   ExitAreaPlate.SetActive(true);
  }

  /* set the collected in UI to Active = Collected*/
  public void collectedOne(int index) {
   collectibleRep[index].material = ActiveCollectiblesMat;
  }

  void SetUpUIRepOfCollectibles(){
     /* UI rep of collectibles*/
    GameObject placeholder = DisplayCollectibles.transform.Find("Placeholder").gameObject;
    if (placeholder == null) { 
      throw new System.NullReferenceException("DisplayCollectibles needs a Child called 'Placeholder'");
    }
    placeholder.SetActive(false);
    

    GameObject collectiblesParent = GameObject.Find("Collectibles_DONOTRENAME");
    collectibleRep = new Image[collectiblesParent.transform.childCount];
    /* find the attached sprites*/
    for (int i = 0; i<collectiblesParent.transform.childCount; i++) {
        Transform child = collectiblesParent.transform.GetChild(i);
        Transform sprite = child.Find("Sprite");
        if (sprite == null) {
          throw new System.NullReferenceException("each child in Collectibles_DONOTRENAME need an other child called 'Sprite' with a spriterender");
        } 
        SpriteRenderer spr = sprite.GetComponent<SpriteRenderer>();
        GameObject placeholderClone = Instantiate(placeholder,DisplayCollectibles.transform);
        RectTransform rt = placeholderClone.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(
          rt.sizeDelta.x * (i),
          0f,
          0f
        );
        placeholderClone.SetActive(true);
        Image im = placeholderClone.transform.GetChild(0).gameObject.GetComponent<Image>();
        im.sprite = spr.sprite;
        im.material = InactiveCollectiblesMat;
        collectibleRep[i] = im; // save it here for better access
    }
  }
}
 