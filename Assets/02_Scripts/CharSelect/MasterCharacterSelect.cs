using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class MasterCharacterSelect : MonoBehaviour
{
    public Transform[] characters;
    private Quaternion[] initRotations;

    private Vector3 leftPos;
    private Vector3 centerPos;
    private Vector3 rightPos;

    public string nameOfAdventure; // for start button

    private int leftChar;
    private int selectedCharacter;
    private int rightChar;

    private Vector3 initScale;
    private Vector3 smallScale;

    [Range(-400.0f, 400.0f)]
    public float RotationSpeed;

    public AudioClip clipClick;
    private AudioSource audioClick;
    [Range(0f,1f)]
    public float clickVolume;

    void Start()
    {
      if(characters.Length == 0)
        throw new Exception("Add characters to CharacterSelect-->Main Camera-->script 'MasterCharacterSelect'.");

      // audio initialization
      audioClick = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
      audioClick.clip = clipClick;
      audioClick.volume = clickVolume;
      audioClick.playOnAwake = false;
      audioClick.loop = false;
      audioClick.pitch = 1.4f;

      // character initialization
      selectedCharacter = 0;
      PlayerSettings.Robot = selectedCharacter;
      rightChar = 1;
      leftChar = characters.Length-1;

      // position
      centerPos = characters[0].position;
      leftPos = new Vector3(centerPos.x-6.5f, centerPos.y, centerPos.z);
      rightPos = new Vector3(centerPos.x+6.5f, centerPos.y, centerPos.z);
      characters[leftChar].position = leftPos;
      characters[rightChar].position = rightPos;

      // scale
      initScale = characters[0].localScale;
      smallScale = initScale - new Vector3(0.5f, 0.5f, 0.5f);
      characters[leftChar].localScale = smallScale;
      characters[rightChar].localScale = smallScale;

      // rotation
      initRotations = new Quaternion[characters.Length];
      for(int i = 0; i < characters.Length; i++){
        initRotations[i] = characters[i].rotation;
        characters[i].gameObject.SetActive(false);
        if(i == selectedCharacter || i == leftChar || i == rightChar)
          characters[i].gameObject.SetActive(true);
      }
    }

    void Update()
    {
      characters[selectedCharacter].Rotate(0f, Time.deltaTime * RotationSpeed, 0f);
    }

    public void switchCharacters(bool left){
      audioClick.Play();
      //update center char
      if(left == true){
        if(selectedCharacter == 0)
          selectedCharacter = characters.Length-1;
        else
          selectedCharacter--;
      }
      else{
        if(selectedCharacter == characters.Length-1)
          selectedCharacter = 0;
        else
          selectedCharacter++;
      }

      //update left char
      if(selectedCharacter-1==-1)
        leftChar = characters.Length-1;
      else
        leftChar = selectedCharacter-1;

      //update right char
      if(selectedCharacter+1 == characters.Length)
        rightChar = 0;
      else
        rightChar = selectedCharacter+1;

      //update all positions and rotations
      for(int i = 0; i < characters.Length; i++){
        characters[i].rotation = initRotations[i];
        characters[i].gameObject.SetActive(false);
      }
      characters[selectedCharacter].gameObject.SetActive(true);
      characters[selectedCharacter].position = centerPos;
      characters[selectedCharacter].localScale = initScale;
      characters[leftChar].gameObject.SetActive(true);
      characters[leftChar].position = leftPos;
      characters[leftChar].localScale = smallScale;
      characters[rightChar].gameObject.SetActive(true);
      characters[rightChar].position = rightPos;
      characters[rightChar].localScale = smallScale;

      PlayerSettings.Robot = selectedCharacter;

    }

    public void StartGame(){
      SceneManager.LoadScene(nameOfAdventure);
    }
}
