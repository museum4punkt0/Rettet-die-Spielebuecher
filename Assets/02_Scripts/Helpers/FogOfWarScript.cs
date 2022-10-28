using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
Handles FOG
*/

public class FogOfWarScript : MonoBehaviour
{
    public Transform player;
    public LayerMask fogLayer;
    public float radius = 5f;
    private float radiusSqr {get {return radius*radius;}}

    public bool FogOn;

    public Color FogColor;

    [Tooltip("The default alpha transparency of unvisited areas (0 is transparent, 1 is opaque")]
    public float DefaultAlpha = .7f;

    public Collider col;
    private Mesh fogMesh;
    private Vector3[] fogVertices;
    private Color[] fogColors;

    [HideInInspector]
    public float[] fogAlphas; // here we save the relevant data

    private Adventure_State state; // this is where state (here for fogAlphas) is stored

    // Start is called before the first frame update
    void Start()
    {
      Init();
    }

    // Update is called once per frame
    void Update()
    {
      
      Vector3 vect = new Vector3(player.transform.position.x+10, player.transform.position.y+5, player.transform.position.z-10);
      Vector3 vect2 = new Vector3(player.transform.position.x+10, player.transform.position.y-5, player.transform.position.z-10);
      // Debug.DrawRay(vect2, vect-vect2);
      Ray r = new Ray(vect2, vect-vect2);
      RaycastHit hit;
       if (col.Raycast(r, out hit, 1000.0f)){
        for(int i = 0; i < fogVertices.Length; i++){
          Vector3 v = transform.TransformPoint(fogVertices[i]);
          float dist = Vector3.SqrMagnitude(v - hit.point);
          if(dist < radiusSqr){
            float alpha = Mathf.Min(fogColors[i].a, dist/radiusSqr);
            // Debug.Log(alpha);
            fogColors[i].a = alpha;
            fogAlphas[i] = alpha;
          }
        }
        UpdateColor();
      }
    }

    void Init(){
      col = GetComponent<Collider>();
      Color blank = Color.white;
      blank.a = 0f;
      fogMesh = GetComponent<MeshFilter>().mesh;
      fogVertices = fogMesh.vertices;
      fogColors = new Color[fogVertices.Length];

      fogAlphas = new float[fogVertices.Length];
      for(int i=0; i < fogColors.Length; i++) {
        fogAlphas[i] = DefaultAlpha; //fogColors[i].a;
        if(FogOn == true)
          fogColors[i] = FogColor;
        else
          fogColors[i] = blank;
      }

      
      /* see if there is something in state and overwrite alphas if so*/
      Scene scene = SceneManager.GetActiveScene();
      state = GameStates.Get_Adventure_State(scene.name);
      // Debug.Log("FOOOOOG " + JsonUtility.ToJson(state.fogAlphas));
      // Debug.Log("FOOOOOG " + JsonHelper.ToJson(state.fogAlphas));
      if(state != null && state.fogAlphas != null && state.fogAlphas.Length > 0 ) {
         for(int i=0; i < state.fogAlphas.Length; i++) {
          if (state.fogAlphas[i] != 0) {
            fogColors[i].a =  state.fogAlphas[i];
            fogAlphas[i] =  state.fogAlphas[i];
          }
           // fogColors[i] = blank;
        }
      
      }

      UpdateColor();
    }

    
    void UpdateColor(){
      fogMesh.colors = fogColors;
      // PlayerSettings.FogAlphas = fogAlphas;
    }

}
