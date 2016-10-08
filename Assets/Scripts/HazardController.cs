using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HazardController : MonoBehaviour {

	public float speed;

	private Rigidbody rb;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.velocity = new Vector3 (0, -1, 0) * speed;
	}
}
