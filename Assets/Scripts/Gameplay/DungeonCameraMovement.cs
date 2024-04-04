using UnityEngine;
using UnityEngine.InputSystem;

public class DungeonCameraMovement : MonoBehaviour
{
	public float moveSpeed = 5f; // Speed of camera movement

	[Header("Box collider bounds")]
	private BoxCollider movementLimits;
	public Vector2 padding;
	public Vector2 offset;
	[Header("Backup bounds")]
	public Vector2 minXZLimits = new Vector2(10f, -12f);
	public Vector2 maxXZLimits = new Vector2(35f, 30f);

	private void Start()
	{
		movementLimits = GameObject.Find("GameplayController").GetComponent<GameplayController>().dungeonBounds;
		if (movementLimits == null)
			Debug.LogWarning("DungeonCameraMovement could not find GameplayController box collider.");
	}
	void Update()
	{
		// Get movement input from keyboard
		Vector2 movementInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

		// Move the camera based on input
		Vector3 moveDirection = new Vector3(-movementInput.x, 0f, movementInput.y);
		Vector3 targetPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

		if (movementLimits != null)
		{
			Bounds limitsBounds = movementLimits.bounds;

			//limitsBounds.Expand(new Vector3(padding.x, 0f, padding.y));

			// increase the bounds size by the padding value and move its center by the offset value
			Vector3 limitsCenter = limitsBounds.center + new Vector3(offset.x, 0f, offset.y);
			limitsBounds = new Bounds(limitsCenter, limitsBounds.size + new Vector3(padding.x, 0f, padding.y));

			targetPosition.x = Mathf.Clamp(targetPosition.x, limitsBounds.min.x, limitsBounds.max.x);
			targetPosition.z = Mathf.Clamp(targetPosition.z, limitsBounds.min.z, limitsBounds.max.z);
		}
		else
		{
			// Clamp target position within limits
			targetPosition.x = Mathf.Clamp(targetPosition.x, minXZLimits.x, maxXZLimits.x);
			targetPosition.z = Mathf.Clamp(targetPosition.z, minXZLimits.y, maxXZLimits.y);
		}

		transform.position = targetPosition;
		//transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
	}
}
