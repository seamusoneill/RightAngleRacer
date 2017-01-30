using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed;
	public Text countText;
	public Text winText;

	private Rigidbody rb;
	private int count;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		count = 0;
		countText.text = "Count: " + count.ToString();
		winText.text = "";
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate()
	{
		float HorizontalMovement = Input.GetAxis("Horizontal");
		float VerticalMovement = Input.GetAxis("Vertical");

		Vector3 force = new Vector3(HorizontalMovement,0.0f,VerticalMovement);

		rb.AddForce(force * speed);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Pickup"))
		{
			other.gameObject.SetActive(false);
			++count;
			countText.text = "Count: " + count.ToString();
			if (count >= 9)
				winText.text = "You Win!";
		}
	}
}
