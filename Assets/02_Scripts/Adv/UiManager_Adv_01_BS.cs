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
public class UiManager_Adv_01_BS : MonoBehaviour
{
  public static UiManager_Adv_01_BS self;

    /* collectibles */
  public GameObject DisplayCollectibles;
  public Material InactiveCollectiblesMat;
  public Material ActiveCollectiblesMat;
  public GameObject collectiblesParent;

  public GameObject BumpIntoWimmeltierPlate;
  public GameObject BumpIntoWimmeltierPlateSolved;
  public GameObject BumpIntoBook;
  public GameObject ExitAreaPlate;
  public GameObject StartUpPlate;

  public int SpaceBetweenIcons;

  public GameObject PlaceHolderCollectiblesUI;

  RectTransform DisplayCollectiblesRectTransform;  // accessing rectrans
  Image[] collectibleRep;
  public GameObject[] icons;


  void Start() {
    self = this;
    SetUpUIRepOfCollectibles();
  }

  void Awake() {
    Messenger<int>.AddListener("COLLECTED_ONE", collectedOne);
    Messenger.AddListener("SHOW_STARTUP_PLATE", showStartUpPlate);
    Messenger.AddListener("BUMP_INTO_WIMMEL", bumpIntoWimmel);
    Messenger.AddListener("BUMP_INTO_WIMMEL_SOLVED",bumpIntoWimmelSolved);
    Messenger.AddListener("BUMP_INTO_BOOK", bumpIntoBook);
    Messenger.AddListener("BUMP_INTO_EXITAREA", bumpIntoExitArea);

  }
  void OnDestroy(){
    Messenger<int>.RemoveListener("COLLECTED_ONE", collectedOne);
    Messenger.RemoveListener("SHOW_STARTUP_PLATE", showStartUpPlate);
    Messenger.RemoveListener("BUMP_INTO_WIMMEL",bumpIntoWimmel);
    Messenger.RemoveListener("BUMP_INTO_WIMMEL_SOLVED",bumpIntoWimmelSolved);
    Messenger.RemoveListener("BUMP_INTO_BOOK", bumpIntoBook);
    Messenger.RemoveListener("BUMP_INTO_EXITAREA", bumpIntoExitArea);
 }

    /* set the collected in UI to Active = Collected*/
  public void bumpIntoBook() {
   BumpIntoBook.SetActive(true);
  }

  public void showStartUpPlate() {
   StartUpPlate.SetActive(true);
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
    icons[index].SetActive(true);
  }


  void SetUpUIRepOfCollectibles(){
    /* UI rep of collectibles*/
    PlaceHolderCollectiblesUI.SetActive(false);
    foreach (var go in icons) {
       go.SetActive(false);
    }
  }
}
