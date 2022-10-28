using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizePosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
            /* finding childs with pos*/
      Transform positionen = transform.Find("Positionen");
      if (positionen != null && positionen.childCount > 0) {
        int r = Random.Range(0,positionen.childCount); // max is exclusive
        transform.position = positionen.GetChild(r).position;
      }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
