using UnityEngine;
using System.Collections;

public class rotate : MonoBehaviour {

	public float speed = 10f;

	void Update () {
		transform.Rotate (Vector3.right, speed * Time.deltaTime);
	}
}
