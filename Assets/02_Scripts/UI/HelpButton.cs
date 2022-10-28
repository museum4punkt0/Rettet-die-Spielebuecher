using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpButton : MonoBehaviour
{
  Button self;
  Button closeHelp;
  public GameObject HelpPlate;
  [Tooltip("Make sure this is a Button GameObject")]
  public Transform CloseHelpButton;// = HelpPlate.transform.Find("CloseHelp");

  
    // Start is called before the first frame update
    void Start()
    { 
      self = gameObject.GetComponent<Button>();
      self.onClick.AddListener(ShowHelp);
      HelpPlate.SetActive(false);
      // set up close
      if (null==CloseHelpButton) {
       throw new System.NullReferenceException("HelpPlate needs a (child) Button (to close itself). Add this Button to CloseHelpButton");
     }
      closeHelp = CloseHelpButton.gameObject.GetComponent<Button>();
      if (null==closeHelp) {
       throw new System.NullReferenceException("HelpPlate needs a (child) Button (to close itself). Add this Button to CloseHelpButton");
     }
     closeHelp.onClick.AddListener(CloseHelp);
    }

    void CloseHelp() {
      // Debug.Log("CLOSE HELP");
      HelpPlate.SetActive(false);
    }

    void ShowHelp() {
      HelpPlate.SetActive(true);
      // Debug.Log("HELP HELP");
    }
}
