using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
These properties can be changed via MaterialPropertyBlock
_RightColor
_TopColor
_FrontColor

_MainColor
_TopLight
_RightLight
_FrontLight

_MainColorBottom
_RightColorBottom
_TopColorBottom
_FrontColorBottom
_RimColorBottom

_GradientStartY
_GradientHeight

_RimColor
_RimPower

_AmbientColor
_AmbientPower

_FogColor
_FogStart
_FogEnd
_FogDensity

_LightMaxDistance
_LightPos

_Alpha
_Cutoff

_LightProbePower

_SpecColorc
_Shininess
_Specular
*/
public class RandomColor : MonoBehaviour {

	void Start () {
		foreach (Transform child in transform) {
			MeshRenderer renderer = child.GetComponent<MeshRenderer>();
			Color c =  new Color(Random.Range(0.0f, 1.0f),Random.Range(0.0f, 1.0f),Random.Range(0.0f, 1.0f));

			MaterialPropertyBlock props = new MaterialPropertyBlock();
			props.SetColor("_MainColor", c);
			props.SetColor("_AmbinetColor", c);
			renderer.SetPropertyBlock(props);
		}
	}

	void Update () {
		
	}
}
