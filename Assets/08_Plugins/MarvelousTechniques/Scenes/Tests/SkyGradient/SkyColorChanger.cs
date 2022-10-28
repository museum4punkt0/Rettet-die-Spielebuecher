using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkyColorChanger : MonoBehaviour {
    public Material mat;
    public Color topStartColor = Color.white; 
    public Color bottomStartDolor = Color.red;
    public Color topEndColor = Color.red; 
    public Color bottomEndColor = Color.white;
    public float timeToChange = 10; // in seconds

    float timeElapsed;

    void Start() {
    }

    void Update() {
		Color sc = Color.Lerp (topStartColor,topEndColor,timeElapsed/timeToChange);
		Color ec = Color.Lerp (bottomStartDolor,bottomEndColor,timeElapsed/timeToChange);

		mat.SetColor("_TopColor", sc);
		mat.SetColor("_BottomColor", ec);
		timeElapsed += Time.deltaTime;
    }
}
