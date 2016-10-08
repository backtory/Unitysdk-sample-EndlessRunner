using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{

	public GameObject gameOverPanel;
	public float boundary;
	public float speed;

	private Rigidbody rb;

	void Start ()
	{
		this.gameOverPanel.SetActive (false);
		this.rb = GetComponent<Rigidbody> ();
	}

	void FixedUpdate ()
	{
		float moveX = Input.GetAxis ("Horizontal");
		rb.velocity = new Vector3 (moveX, 0.0f, 0.0f) * speed;
		rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, -boundary, boundary),
			rb.position.y,
			rb.position.z
		);
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Hazard") {
			Destroy (this.gameObject);
			Destroy (other.gameObject);

			// Announce "Game Over!"
			gameOverPanel.SetActive (true);
		}
	}
}
