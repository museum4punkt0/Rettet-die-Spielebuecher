using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoWormPlate : MonoBehaviour
{
    public GameObject UIPlateToShow;
    [Tooltip("This should get set to always true, because otherwise there might be dead-loops with pathfinding")]
    public bool HideAfterHit;

    public void showPlate() {
      if (UIPlateToShow != null) {
        UIPlateToShow.SetActive(true);
      }
      if (HideAfterHit) {
        gameObject.SetActive(false);
      }
    }

}