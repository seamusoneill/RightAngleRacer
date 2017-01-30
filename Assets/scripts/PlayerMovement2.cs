using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour {

	public float speed = 6.0f;

	Vector3 movement;
	int floorMask;
	float camRayLength = 100f;
	Vector3 direction;

	void Awake()
	{
		floorMask = LayerMask.GetMask("Floor");
		direction = Vector3.zero;
	}

	void FixedUpdate()
	{
		//float h = Input.GetAxisRaw("Horizontal");
		//float v = Input.GetAxisRaw("Vertical");
		if (Input.GetKeyDown(KeyCode.Space)) {
			direction = transform.forward;
		}
		Move ();
		Turning();
	}

	void ChangeDirection()
	{

	}

	void Move()
	{
		transform.position += direction * speed * Time.deltaTime;	
	}

	void Turning()
	{
		Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit floorHit;

		if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
		{
			Vector3 playerToMouse = floorHit.point - transform.position;
			playerToMouse.y = 0.0f;

			Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
			transform.rotation = newRotation;
		}
	}
}
