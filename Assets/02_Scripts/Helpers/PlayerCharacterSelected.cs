using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCharacterSelected : MonoBehaviour
{
    public GameObject[] characters;
    private int selectedCharacter;
    // Start is called before the first frame update
    void Start()
    {
      if(characters.Length == 0) {
        throw new Exception("Add characters to CharacterSelect-->Main Camera-->script 'PlayerCharacterSelected'.");
      }
        
    }


}
