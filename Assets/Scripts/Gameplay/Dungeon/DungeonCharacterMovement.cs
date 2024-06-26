using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DungeonCharacterMovement : MonoBehaviour
{
	Rigidbody body;
	public GameObject testGoal;
	public bool testMove = false;
	[SerializeField] private float moveSpeed = 100;
	[SerializeField] private float rotationSpeed = 10f;
	[SerializeField] private float finalStoppingDistance = 1f;
	[HideInInspector] public DungeonCharacterManager manager;
	[HideInInspector] public DungeonNavigationSystem navigationSystem;
	[HideInInspector] public float cellSize;
	private DungeonCell[,] knownCells; // this characters vesrion of the dungeon grid, only containing the cells they know about

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
		manager = GetComponentInParent<DungeonCharacterManager>();
		navigationSystem = GetComponentInParent<DungeonCharacterManager>().navigationSystem;
		cellSize = GetComponentInParent<DungeonCharacterManager>().navigationSystem.cellSize;
		testGoal = GameObject.Find("testGoal");
	}
	private void Start()
	{
		knownCells = new DungeonCell[navigationSystem.width, navigationSystem.height];
		// pass current position to a function in DungeonNavSystem which returns a dungeon cell with a 2D array position so i can add it to knownCells


		if (testGoal != null && testMove)
			MoveTo(testGoal.transform.position);
	}
	public void DoUpdate()
	{

	}
	// to move the character i set isMoving to true, some other logic rotates the character towards its target. this is bad but it is what it is
	private bool isMoving = false;
	public void DoFixedUpdate()
	{
		if (isMoving)
			body.AddForce(transform.forward * moveSpeed * Time.deltaTime, ForceMode.Force);
	}
	// order the character to move to a position on the grid, starts the coroutine which handles movement
	public void MoveTo(Vector3 targetPos)
	{
		// move to each position in turn until at goal
		StartCoroutine(MoveToTargetPositions(targetPos));
	}

	private List<Vector3> debugPath;
	// construct a path to the target position
	private IEnumerator MoveToTargetPositions(Vector3 targetPos)
	{
		isMoving = true;
		bool reachedGoal = false;
		int currentPositionIndex = 0;
		float baseStoppingDistance = 0.7f; // used for every position but the final one

		while (!reachedGoal)
		{
			// construct a path
			List<Vector3> path = navigationSystem.ConstructPath(transform.position, targetPos);
			debugPath = path;

			if (path == null || path.Count == 0)
			{
				Debug.LogWarning($"Couldnt get a path, movement to ({targetPos}) failed.");
				yield break;
			}

			Vector3 targetPosition = path[currentPositionIndex];

			// Rotate towards the target position
			Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

			float stoppingDistance = currentPositionIndex == path.Count - 1 ? finalStoppingDistance : baseStoppingDistance;
			// Check if we are within range of the target position
			if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
			{
				// Move to the next position
				currentPositionIndex++;

				// Check if we've reached the final position
				if (currentPositionIndex >= path.Count)
				{
					isMoving = false;
					reachedGoal = true;
					Debug.Log("Reached the final position!");
					//spawn confetti
					ParticleManager.SpawnParticle(transform.position + new Vector3(0, 3, 0), ParticleManager.Particle.Confetti);
					yield break;
				}
			}

			yield return null;
		}
	}

	public void StartBoundsCheck(Vector3Int cellPosition)
	{
		StartCoroutine(CheckCharacterBounds(cellPosition));
	}

	// Coroutine to check if the character leaves the cell bounds
	private IEnumerator CheckCharacterBounds(Vector3Int cellPosition)
	{
		while (true)
		{
			// Check if the character has left the bounds of the cell
			if (!IsWithinBounds(cellPosition, transform.position))
			{
				// notify the manager that we left the cell
				navigationSystem.RemoveCharacterFromCellPosition(cellPosition, this.gameObject);

				// stop the coroutine
				yield break;
			}
			// Wait for the next frame
			yield return null;
		}
	}

	private bool IsWithinBounds(Vector3Int cellPosition, Vector3 position)
	{
		return position.x >= cellPosition.x && position.x <= cellPosition.x + cellSize && position.z >= cellPosition.z && position.z <= cellPosition.z + cellSize;
	}

	public void MoveTo2(Vector3 targetPos)
	{
		// construct a path
		List<Vector3> path = navigationSystem.ConstructPath(transform.position, targetPos);

		if (path == null || path.Count == 0)
		{
			Debug.LogWarning($"Couldnt get a path, movement to ({targetPos}) failed.");
			return;
		}

		// move to each position in turn until at goal
		StartCoroutine(MoveToTargetPositions2(path));
	}

	private IEnumerator MoveToTargetPositions2(List<Vector3> targetPositions)
	{
		isMoving = true;
		bool reachedGoal = false;
		int currentPositionIndex = 0;
		float baseStoppingDistance = 0.7f; // used for every position but the final one
		while (!reachedGoal && currentPositionIndex < targetPositions.Count)
		{
			Vector3 targetPosition = targetPositions[currentPositionIndex];

			// Rotate towards the target position
			Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

			// Move towards the target position
			//body.velocity = transform.forward * moveSpeed * Time.deltaTime;

			float stoppingDistance = currentPositionIndex == targetPositions.Count - 1 ? finalStoppingDistance : baseStoppingDistance;
			// Check if we are within range of the target position
			if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
			{
				// Move to the next position
				currentPositionIndex++;

				// Check if we've reached the final position
				if (currentPositionIndex >= targetPositions.Count)
				{
					isMoving = false;
					reachedGoal = true;
					Debug.Log("Reached the final position!");
					//spawn confetti
					ParticleManager.SpawnParticle(transform.position + new Vector3(0, 3, 0), ParticleManager.Particle.Confetti);
				}
			}

			yield return null;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (debugPath != null && debugPath.Count > 0)
		{
			// Draw lines between each pair of positions
			Gizmos.color = Color.blue;
			for (int i = 0; i < debugPath.Count - 1; i++)
			{
				Gizmos.DrawLine(debugPath[i], debugPath[i + 1]);
			}

			// Draw spheres at each position
			Gizmos.color = Color.red;
			foreach (Vector3 position in debugPath)
			{
				Gizmos.DrawSphere(position, 0.1f);
			}
		}
	}
}