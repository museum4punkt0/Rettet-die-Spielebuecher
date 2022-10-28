using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using DUCK.Tween;
using DUCK.Tween.Easings;


public class MasterCharacterSelect_LFK : MonoBehaviour
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

    private bool AnimationIsOVer = true;

    [
    Range(-400.0f, 400.0f),
    Tooltip("The speed the Character rotates around itself")
    ]
    public float RotationSpeed;
    
    [
    Range(0.1f, 3.0f),
    Tooltip("The time the Wheel rotates (in sec)")
    ]
    public float PlateRotationSpeed;
    [
      Range(0.1f, 3.0f),
      Tooltip("The time the Characters up/downscale (in sec)")
    ]
    public float ScaleSpeed;

    public AudioClip clipClick;
    private AudioSource audioClick;
    [Range(0f,1f)]
    public float clickVolume;

    /* LFK */
    public Transform turningPlate; 
    float rotateStep = 10f; // degrees the plate rotates each click


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
      // leftPos = new Vector3(centerPos.x-6.5f, centerPos.y, centerPos.z);
      // rightPos = new Vector3(centerPos.x+6.5f, centerPos.y, centerPos.z);
      // characters[leftChar].position = leftPos;
      // characters[rightChar].position = rightPos;

      // scale
      initScale = characters[0].localScale;
      // Debug.Log(initScale);
      // smallScale = initScale - new Vector3(0.5f, 0.5f, 0.5f);
      // characters[leftChar].localScale = smallScale;
      // characters[rightChar].localScale = smallScale;

      // rotation
      initRotations = new Quaternion[characters.Length];

      for(int i = 0; i < characters.Length; i++){
        // initRotations[i] = characters[i].rotation;
        characters[i].gameObject.SetActive(false);
        // if(i == selectedCharacter || i == leftChar || i == rightChar)
        if(i == selectedCharacter)
          characters[i].gameObject.SetActive(true);
      }
      /* lfk */
      rotateStep = 360f / characters.Length;
      //  var upscale = new ScaleAnimation(characters[selectedCharacter].gameObject, characters[selectedCharacter].localScale, 
      //   characters[selectedCharacter].localScale*2, 2f, Ease.Back.Out);
      // upscale.Play(() => { 
      //   // Debug.Log("It's complete - start"); 
      //   });
    }

    void Update()
    {
      // rotate the character around itself
      characters[selectedCharacter].Rotate(0f, Time.deltaTime * RotationSpeed, 0f);
    }

    public void switchCharacters(bool left){
      /* wait till anim is over*/
      if (!AnimationIsOVer) {return;}
      AnimationIsOVer = false;
      // Debug.Log(AnimationIsOVer);


      int formerIndex=selectedCharacter;
      audioClick.Play();
      Vector3 finalRot = new Vector3(0f,0f,0f);

      // update center char
      if(left == true){
        finalRot = new Vector3(turningPlate.eulerAngles.x,turningPlate.eulerAngles.y+rotateStep,turningPlate.eulerAngles.z) ;
        if(selectedCharacter == 0)
          selectedCharacter = characters.Length-1;
        else
          selectedCharacter--;
      }
      else{
        finalRot = new Vector3(turningPlate.eulerAngles.x,turningPlate.eulerAngles.y-rotateStep,turningPlate.eulerAngles.z) ;
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

      //update visibility
      for(int i = 0; i < characters.Length; i++){
        characters[i].gameObject.SetActive(false);
      }
      if (left) {
         characters[rightChar].gameObject.SetActive(true);
      } else {
         characters[leftChar].gameObject.SetActive(true);
       }
      characters[selectedCharacter].gameObject.SetActive(true);
      // characters[leftChar].gameObject.SetActive(true);

      /* do turning plate animation*/
      var plateRotateAnim = new RotateAnimation(turningPlate.gameObject, turningPlate.eulerAngles, finalRot, PlateRotationSpeed);
       /* we are waiting for this animation to finish before handling next animation*/
      plateRotateAnim.Play(() => { 
        AnimationIsOVer = true;
        characters[leftChar].gameObject.SetActive(false);
        characters[rightChar].gameObject.SetActive(false);
       });
      
      // characters[rightChar].gameObject.SetActive(true);

      /* upscale current character */
      // var upscale = new ScaleAnimation(characters[selectedCharacter].gameObject, characters[selectedCharacter].localScale, characters[selectedCharacter].localScale*2, ScaleSpeed, Ease.Back.In);
      // upscale.Play();
      if (selectedCharacter != formerIndex) {
        // var downscale = new ScaleAnimation(characters[formerIndex].gameObject, characters[formerIndex].localScale, characters[formerIndex].localScale*0.5f, ScaleSpeed, Ease.Back.In);
        /* we are waiting for this animation to finish before handling next animation*/
        // downscale.Play(() => { AnimationIsOVer = true; });
      }


      PlayerSettings.Robot = selectedCharacter;

    }

    public void StartGame(){
      Charakter_State state = new Charakter_State();
      state.characterID = selectedCharacter;
      GameStates.Set_Charakter_State(state);
      SceneManager.LoadScene(nameOfAdventure);
    }


    /* call this in update
     
     */
    // bool TweenRotate( float rotY, float starttime, float EulerTargetAngle) {
     // float rotateY = end.x>start.x ? 180 : 90;
     // rotateY = end.z>start.z ? 90 : 180;
     // if (start.z>end.z && end.x==start.x) rotateY = 90;
     // if(PlayerSettings.Robot != 0){
     //    rotateY = end.x>start.x ? 90 : 360;
     //    rotateY = end.z>start.z ? 360 : 90;
     //    if (start.z>end.z && end.x==start.x) rotateY = 180;
     //    if (start.z == end.z && start.x > end.x) rotateY = 270;
     // }
      // rotateY = Input.GetAxisRaw("Vertical")!= 0 ? 90 : 180;
    // transform.rotation = Quaternion.Euler(0.0f, rotateY, 0.0f);
    //  // Distance moved = time * speed.
    // float distCovered = (Time.time - starttime) * moveSpeed;


    //     // Fraction of journey completed = current distance divided by total distance.
    // float fracJourney = distCovered / journeyLength;

    // if (fracJourney > 1f) {
    //     transform.position =  new Vector3(end.x,transform.position.y,end.z);
    //     return false;
    // }
    //     // Set our position as a fraction of the distance between the markers.
    // Vector3 newPos = Vector3.LerpUnclamped(start, end, fracJourney);
    // if ( !float.IsNaN(newPos.x) && !float.IsNaN(newPos.z) ) {
    //     transform.position =  new Vector3(newPos.x,transform.position.y,newPos.z);
    // }
    // return true;
// }
}
