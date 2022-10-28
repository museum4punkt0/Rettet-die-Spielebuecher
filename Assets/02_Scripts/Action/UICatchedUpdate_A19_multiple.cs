using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/*<summary>
 * Listen to Messenger Event CATCH_KORREKT 
 * and uses the sprite in Slotcontroller whatToCatch to take the sprite from there
 * and display it in rectTransform
 * LFK 2019-09-08_10.23.27: Code logic is kind of weird, which is due to the incremental develepmont. 
 * Anyway there are opportunities to make this better.
 * 
 *</summary>*/
 public class UICatchedUpdate_A19_multiple : MonoBehaviour
 {
   public static UICatchedUpdate_A19_multiple singleton;
   
    [Tooltip("Since positioning UI-Elements is not fun in code, set a gameobject here, which is used as a placeholder")]
   public RectTransform ItemToDuplicate;
   public SlotsController_A19_multiple SlotManager;

   public Material inActiveMat;
   public Material defaultMat;
   public Material currentQuestMat;

   [Tooltip("How much the current item/quest is upscaled")]
   public float scaleFactor = 1.5f;
   private Vector2[] defaultScales;

   private int currentQuestInternal = 0;
   private bool onceDone = false;

  [System.Serializable]
  public class WhatToCatchUI {
     public string name;
     public Sprite sprite;
     public int count;
   }
   [Tooltip("Set what to catch to have the level done")]
   public WhatToCatchUI[] whatToCatchUI;
   public WhatToCatchUI[] baseWhatToCatchUI;


  void Start()
  {
    var state = GameStates.Get_Action_State(SceneManager.GetActiveScene().name);
    if (state.whatToCatchUI != null && 
        state.whatToCatchUI.Length > 0 &&
        (state.whatToCatchUI[0].count > 0 || state.whatToCatchUI[1].count > 0))
    {
      whatToCatchUI = state.whatToCatchUI;
      if(whatToCatchUI[0].count < whatToCatchUI[1].count)
      {
        SlotManager.currentQuestID = 1;
      }
    }

    defaultScales = new Vector2[whatToCatchUI.Length];
    /* add images and position the UI-Elements based on elements in WhatToCatchUI*/
    for (int i = 0; i < whatToCatchUI.Length; i++) {
      GameObject clone = Instantiate(ItemToDuplicate.gameObject, transform);
      RectTransform rt = clone.GetComponent<RectTransform>();
      rt.localPosition = new Vector3(
        ItemToDuplicate.localPosition.x + ItemToDuplicate.sizeDelta.x * (i),
        ItemToDuplicate.localPosition.y,
        0f
      );
      Transform ic = clone.transform.Find("Image");
      RectTransform icrt = ic.gameObject.GetComponent<RectTransform>();
      defaultScales[i] = icrt.localScale;
      Image ici = ic.gameObject.GetComponent<Image>();
      Transform t = clone.transform.Find("Text");
      Text tt = t.gameObject.GetComponent<Text>();
      tt.text = "x " + whatToCatchUI[i].count;
      ici.sprite = whatToCatchUI[i].sprite;
      ici.enabled = true;
      clone.name = whatToCatchUI[i].name;
    }
    ItemToDuplicate.gameObject.SetActive(false);
  }

   void Awake()
   {
     if (singleton == null)
     { 
       singleton = this;
     }
     else
     {
       Destroy(this);
     }

     baseWhatToCatchUI = whatToCatchUI;

     // Debug.Log("UICatchedUpdate_A19_multiple AWAKE !!!");
    Messenger<string>.AddListener("CATCHED_ONE", CatchedOne);
  }
  void OnDestroy()
  {
    singleton = null;
    
    // Debug.Log("UICatchedUpdate_A19_multiple Destroy Q!!!!");
    Messenger<string>.RemoveListener("CATCHED_ONE", CatchedOne);
  }

 // void UpdateUI(int current) {
 //     slotname == whatToCatchUI[current].name;
 //     Transform ss = transform.Find(slotname);
 //     Transform t = ss.Find("Text");
 //     int count = whatToCatchUI[i].count - 1;
 //     Text tt = t.GetComponent<Text>();
 //     if (count>=1) {
 //      tt.text = "x " + (count);
 //      whatToCatchUI[i].count = count;
 //    } else {
 //      Transform im = ss.Find("Image");
 //      Image imi = im.GetComponent<Image>();
 //      imi.material = inActiveMat;
 //      whatToCatchUI[i].count = 0;
 //      tt.text = "";
 //    }
 // }

 /* check if correct catch (again) (not nice) 
    than decrement count (when matching)
    and fire Quest-Complete when all on 0
    name comes from Slots-Controller (What to catch) and contains the name of the Gameobject(!) - with cloned
  */
 void CatchedOne(string name) {
    string cleaned = name.Replace("(Clone)","");
    string relevantOne = SlotManager.whatToCatch[currentQuestInternal].gameObject.name;

    /* do nothing if the catch is wrong */
     if (cleaned != relevantOne) {
       return;
     }
     Messenger.Broadcast("CATCH_KORREKT");

     for(int i = 0; i < whatToCatchUI.Length; i++) {
       string slotname = SlotManager.whatToCatch[currentQuestInternal].name;
       // string slotname = SlotManager.whatToCatch[SlotManager.currentQuestID].name;
       // Debug.Log("slotname " + slotname);
       // Debug.Log("slotname UI " + whatToCatchUI[currentQuestInternal].name);
       if (slotname == whatToCatchUI[i].name) {
          Transform ss = transform.Find(slotname);
          Transform t = ss.Find("Text");
          int count = whatToCatchUI[i].count - 1;
          Text tt = t.GetComponent<Text>();
          if (count>=1) {
            tt.text = "x " + (count);
            whatToCatchUI[i].count = count;
          } else {
            Transform im = ss.Find("Image");
            Image imi = im.GetComponent<Image>();
            imi.material = inActiveMat;
            whatToCatchUI[i].count = 0;
            tt.text = "";
          }
       }
     }
     bool alldone = true;
     for(int i = 0; i < whatToCatchUI.Length; i++) {
      // Debug.Log("MOFU "+ i + ": "+ whatToCatchUI[i].count);
      if (whatToCatchUI[i].count != 0) {
        alldone = false;
        break;
      }
     }
      // Debug.Log("alldone " + alldone);
     if (alldone) {
        Messenger<float>.Broadcast("QUEST_COMPLETED", Time.time);
     }
  }
  
 Image getCurrentRep(int slotID) {
    for(int i = 0; i < whatToCatchUI.Length; i++) {
       string slotname = SlotManager.whatToCatch[slotID].name;
       // Debug.Log("slotname " + slotname);
       if (slotname == whatToCatchUI[i].name) {
          Transform ss = transform.Find(slotname);
          Transform im = ss.Find("Image");
          Image imi = im.GetComponent<Image>();
          return imi;
       }
    }
    return null;
  }



  // Update is called once per frame
  void Update()
  {
     
     // currentQuestId incremeted, we need to highlight the currently quested object 
     if(currentQuestInternal != SlotManager.currentQuestID || onceDone == false ) {
       Image imi = getCurrentRep(SlotManager.currentQuestID);
       if (imi != null) {
         imi.material = currentQuestMat;
         RectTransform rt = imi.gameObject.GetComponent<RectTransform>();
         rt.localScale = new Vector2(scaleFactor,scaleFactor);
         // rt.pivot = new Vector2(scaleFactor,0f);
         
       }
       // reset the one before
       if (onceDone == true) {
         imi = getCurrentRep(currentQuestInternal);
         if (imi != null) {
           imi.material = defaultMat;
           RectTransform rt = imi.gameObject.GetComponent<RectTransform>();
           rt.localScale = defaultScales[currentQuestInternal];
           // rt.anchoredPosition = new Vector2(0f,0f);
         }
       }
       currentQuestInternal = SlotManager.currentQuestID;
       onceDone = true;

     }
  }
}
