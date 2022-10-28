//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DistanceLightMulti : MonoBehaviour {

	public List<Transform> lights = new List<Transform>();

	public float maxDistance =5.0f;
	public bool additiveBlending =false;
	public Texture rampTexture = null;
	List<Material> materials = new List<Material>();

	Vector4[] positions = new Vector4[10];
	Texture lastRampTexture = null;

	float lastDistance = float.MaxValue;
	bool lastBlending = false;
	int lightsCount = 0;

	// Use this for initialization
	void Start () {
		findMaterials();
	}

	public void distanceLightChanged(){
		findMaterials();
	}

	void checkMaterial(Material mat){
		if (mat && !materials.Contains (mat)) {
			if (mat.shader != null) {
				if(mat.shader.name.Contains("Kirnu/Marvelous/") &&
					(mat.shader.name.Contains("DistanceLight") || mat.shader.name.Contains("CustomLightingMaster"))){
					materials.Add(mat);
				}
			}
		}
	}

	void findMaterials(){
		materials.Clear ();
		Renderer[] arrend = (Renderer[])Resources.FindObjectsOfTypeAll(typeof(Renderer));
		foreach (Renderer rend in arrend) {
			if (rend != null) {
				foreach (Material mat in rend.sharedMaterials) {
					checkMaterial (mat);
				}
			}
		}

		Terrain[] terrains = Terrain.activeTerrains;
		foreach(Terrain terrain in terrains){
			Material mat = terrain.materialTemplate;
			checkMaterial (mat);
		}

		updateMaterials ();
	}

	void updatePositions(Material m) {
		lightsCount = lights.Count;
		m.SetFloat ("_LightPositionsCount", lights.Count);

		for (int i = 0; i < lights.Count && i < 10; i++) {
			positions [i] = lights [i].position;
		}
		m.SetVectorArray ("_LightPositions", positions);
	}

	void updateMaterials(){
		foreach(Material m in materials){
			updatePositions (m);
			if (lightsCount != lights.Count) {
				
			}
			if(m && m.HasProperty("_LightMaxDistance") && lastDistance != maxDistance){
				m.SetFloat("_LightMaxDistance",maxDistance);
				updatePositions (m);
			}
			if(m && m.HasProperty("_LightRampTexture") && rampTexture && lastRampTexture != rampTexture){
				m.SetTexture("_LightRampTexture",rampTexture);
				updatePositions (m);
			}
			if(m && lastBlending != additiveBlending){
				updatePositions (m);
				if(additiveBlending){
					m.EnableKeyword("DIST_LIGHT_ADDITIVE");
				}
				else{
					m.DisableKeyword("DIST_LIGHT_ADDITIVE");
				}
			}
		}
		lastDistance = maxDistance;
		lastRampTexture = rampTexture;
		lastBlending = additiveBlending;
	}

	void Update () {
		updateMaterials ();
	}
}
