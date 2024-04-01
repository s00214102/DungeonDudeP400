using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
	public float moveSpeed = 5f; // Speed of camera movement
	public Vector2 minXZLimits = new Vector2(10f, -12f);
	public Vector2 maxXZLimits = new Vector2(35f, 30f);
	void Update()
	{
		// Get movement input from keyboard
		Vector2 movementInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

		// Move the camera based on input
		Vector3 moveDirection = new Vector3(-movementInput.x, 0f, movementInput.y);
		Vector3 targetPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

		// Clamp target position within limits
		targetPosition.x = Mathf.Clamp(targetPosition.x, minXZLimits.x, maxXZLimits.x);
		targetPosition.z = Mathf.Clamp(targetPosition.z, minXZLimits.y, maxXZLimits.y);

		transform.position = targetPosition;
		//transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
	}
}
