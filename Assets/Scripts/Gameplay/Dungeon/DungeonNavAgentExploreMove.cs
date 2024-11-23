using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Testing the exploration movement using the seeable cell calculations of the DungeonNavigationSystem.
/// The goal is for the agent to find the goal without me telling it where the goal is.
/// It does this by moving to cells it knows about which border cells it doesnt know about.
/// It will repeat this until it sees the goal. 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DungeonNavAgentExploreMove : MonoBehaviour
{
	//TODO create a system for assigning ids to agents, handled by the DungeonCharacterManager. just setting it to 0 for now
	int agentId = 0;
	Rigidbody body;
	//public GameObject testGoal;
	//public bool testMove = false;
	[SerializeField] private float moveSpeed = 100;
	[SerializeField] private float rotationSpeed = 10f;
	[SerializeField] private float finalStoppingDistance = 0.2f; // TODO use a different value here for position/object move
	[HideInInspector] public DungeonCharacterManager manager;
	[HideInInspector] public DungeonNavigationSystem navigationSystem;
	[HideInInspector] public float cellSize;

	private DungeonCell[,] knownCells; // this characters version of the dungeon grid, only containing the cells they know about
	private List<DungeonCell> knownCellsList = new();

	private Action destinationReached;

	public void Init(DungeonCharacterManager manager, DungeonNavigationSystem navSystem)
	{
		Debug.Log($"Initializing DungeonNavAgentExploreMove {this.gameObject.name}.");

		body = GetComponent<Rigidbody>();
		this.manager = manager;
		this.navigationSystem = navSystem;

		cellSize = GetComponentInParent<DungeonCharacterManager>().navigationSystem.cellSize;

		knownCells = new DungeonCell[navigationSystem.width, navigationSystem.height];

		// add the current/starting cell the agent is standing in now
		// get current position as cellXZ grid position
		Vector2 startingCellPos = navigationSystem.GetCellCoordsFromWorldPos(this.transform.position);
		knownCells[(int)startingCellPos.x, (int)startingCellPos.y] = navigationSystem.dungeonGrid[(int)startingCellPos.x, (int)startingCellPos.y];
		AddOrUpdateDebugSphere(knownCells[(int)startingCellPos.x, (int)startingCellPos.y].worldPositionCenter, Color.yellow, 60.0f);

		// pass current cell position to a function in DungeonNavSystem which returns a dungeon cell with a 2D array position so i can add it to knownCells
		List<DungeonCell> newCells = navigationSystem.GetSeeableCellsFromPosition(transform.position);
		knownCellsList = newCells; // just used for easier iteration over the agents known cells 

		UpdateKnownCells(newCells);
		MoveTo(PickAPriorityCellToMoveTo());

		destinationReached = () =>
		{
			Vector3 priorityCellPosition = PickAPriorityCellToMoveTo();
			if (priorityCellPosition != Vector3.zero)
			{
				MoveTo(priorityCellPosition);
			}
			else
			{
				Debug.Log("No more priority cells to move to.");
			}
		};
	}

	public void UpdateKnownCells(List<DungeonCell> newCells)
	{
		// add the new cells
		foreach (var cell in newCells)
		{
			knownCells[cell.xPos, cell.zPos] = cell;

			if (!knownCellsList.Contains(cell))
				knownCellsList.Add(cell);

			AddOrUpdateDebugSphere(cell.worldPositionCenter, Color.yellow, 60.0f);
		}
		// reset priority on all known cells
		foreach (var cell in knownCells)
		{
			if (cell != null)
				cell.isPriority = false;
		}
		// set move priority on cells on the frontier
		// for each cell, check adjacents cells for
		// cells that are not already in the knownCells list that are also isWalkable = true

		// Set move priority on cells on the frontier
		for (int x = 0; x < knownCells.GetLength(0); x++)
		{
			for (int z = 0; z < knownCells.GetLength(1); z++)
			{
				var currentCell = knownCells[x, z];
				if (currentCell == null) continue; // Skip empty slots in the knownCells array

				// Check adjacent cells
				var adjacentOffsets = new (int, int)[]
				{
				(-1, 0), (1, 0), // Left, Right
                (0, -1), (0, 1)  // Down, Up
				};

				foreach (var (dx, dz) in adjacentOffsets)
				{
					int adjacentX = x + dx;
					int adjacentZ = z + dz;

					// Ensure adjacent cell indices are within bounds
					if (adjacentX >= 0 && adjacentX < navigationSystem.width &&
						adjacentZ >= 0 && adjacentZ < navigationSystem.height)
					{
						var adjacentCell = navigationSystem.dungeonGrid[adjacentX, adjacentZ];

						// Check if the adjacent cell is walkable and not already in knownCells
						if (adjacentCell != null && adjacentCell.isWalkable &&
					   knownCells[adjacentX, adjacentZ] == null)
						{
							// Mark the current cell as priority
							currentCell.isPriority = true;
							AddOrUpdateDebugSphere(currentCell.worldPositionCenter + new Vector3(0, 0, 0), Color.green, 60.0f); // Visualize priority cells
							break; // No need to check more adjacent cells for this current cell
						}
					}
				}
			}
		}
	}

	private Vector3 PickAPriorityCellToMoveTo()
	{
		// just get any priority cell from the agents known cells and pass back its position
		foreach (var cell in knownCells)
		{
			//TODO pick the priority cell using some method or technique. e.g. pick the closest one. this could be tied to a hero trait.
			if (cell != null && cell.isPriority)
				return cell.worldPositionCenter;
		}
		//TODO a cell might be at (0,0,0) so maybe pass back a bool too, see NavigationSystem
		return Vector3.zero;
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

	private Coroutine currentMoveCoroutine;
	// order the character to move to a position on the grid, starts the coroutine which handles movement
	public void MoveTo(Vector3 targetPos)
	{
		if (currentMoveCoroutine != null)
		{
			StopCoroutine(currentMoveCoroutine);
		}
		// move to each position in turn until at goal
		currentMoveCoroutine = StartCoroutine(MoveToTargetPositions(targetPos));
	}

	// construct a path to the target position
	private IEnumerator MoveToTargetPositions(Vector3 targetPos)
	{
		Debug.Log($"Moving agent {this.gameObject.name} to {targetPos}");
		isMoving = true;
		bool reachedGoal = false;
		int currentPositionIndex = 0;
		float baseStoppingDistance = 0.7f; // used for every position but the final one
		int pathStep = 0; // used to lessen the number of calls to create a new path.
		List<Vector3> path = null;
		navigationSystem.CreatePathToTargetForAgent(agentId, transform.position, targetPos);
		path = navigationSystem.agentPaths[agentId];

		while (!reachedGoal)
		{
			//TODO construct a path once every X frames
			if (pathStep >= 60) // certain number of frames to skip before getting a new path
			{
				navigationSystem.CreatePathToTargetForAgent(agentId, transform.position, targetPos);
				path = navigationSystem.agentPaths[agentId]; // update the path incase things have changed, e.g. other agents positions may block the path
				pathStep = 0;
			}
			pathStep++;


			if (path == null || path.Count == 0)
			{
				Debug.LogWarning($"Couldnt get a path, movement to ({targetPos}) failed.");
				yield break;
			}

			debugPath = path;
			Vector3 targetPosition = path[currentPositionIndex];

			// Rotate towards the target position
			Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

			//TODO handle moving to a position differently to moving to an object. move to position directly, move to object and stop when close enough.
			//float stoppingDistance = currentPositionIndex == path.Count - 1 ? finalStoppingDistance : baseStoppingDistance;
			float stoppingDistance = 0;
			// Check if we are within range of the target position
			if (Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
			{
				// Move to the next position
				currentPositionIndex++;
			}
			// Check if we've reached the final position
			if (currentPositionIndex >= path.Count - 1)
			{
				isMoving = false;
				reachedGoal = true;
				//spawn confetti
				//TODO stack overflow problem with the particle manager FIX
				//ParticleManager.SpawnParticle(transform.position + new Vector3(0, 3, 0), ParticleManager.Particle.Confetti);

				//TODO for now im just passing the agents current list of known cells, this will just reprioritise them
				// move to a new priority cell.
				destinationReached?.Invoke();
				yield break;
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

	// public void MoveTo2(Vector3 targetPos)
	// {
	// 	// construct a path
	// 	List<Vector3> path = navigationSystem.GetPathToTargetForAgent(transform.position, targetPos);

	// 	if (path == null || path.Count == 0)
	// 	{
	// 		Debug.LogWarning($"Couldnt get a path, movement to ({targetPos}) failed.");
	// 		return;
	// 	}

	// 	// move to each position in turn until at goal
	// 	StartCoroutine(MoveToTargetPositions2(path));
	// }

	// private IEnumerator MoveToTargetPositions2(List<Vector3> targetPositions)
	// {
	// 	isMoving = true;
	// 	bool reachedGoal = false;
	// 	int currentPositionIndex = 0;
	// 	float baseStoppingDistance = 0.7f; // used for every position but the final one
	// 	while (!reachedGoal && currentPositionIndex < targetPositions.Count)
	// 	{
	// 		Vector3 targetPosition = targetPositions[currentPositionIndex];

	// 		// Rotate towards the target position
	// 		Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
	// 		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

	// 		// Move towards the target position
	// 		//body.velocity = transform.forward * moveSpeed * Time.deltaTime;

	// 		float stoppingDistance = currentPositionIndex == targetPositions.Count - 1 ? finalStoppingDistance : baseStoppingDistance;
	// 		// Check if we are within range of the target position
	// 		if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
	// 		{
	// 			// Move to the next position
	// 			currentPositionIndex++;
	// 		}
	// 		// Check if we've reached the final position
	// 		if (currentPositionIndex >= targetPositions.Count)
	// 		{
	// 			isMoving = false;
	// 			reachedGoal = true;
	// 			Debug.Log("Reached the final position!");
	// 			//spawn confetti
	// 			ParticleManager.SpawnParticle(transform.position + new Vector3(0, 3, 0), ParticleManager.Particle.Confetti);
	// 		}
	// 		yield return null;
	// 	}
	// }

	//#if UNITY_EDITOR
	private class DebugSphere
	{
		public Vector3 position;
		public Color color;
		public float expirationTime;
	}
	private List<DebugSphere> knownCellsSpheres = new List<DebugSphere>();
	private void AddOrUpdateDebugSphere(Vector3 position, Color color, float duration)
	{
		// Check if a debug sphere already exists for this position
		var existingSphere = knownCellsSpheres.FirstOrDefault(sphere => sphere.position == position);

		if (existingSphere != null)
		{
			// Update the color and expiration time of the existing sphere
			existingSphere.color = color;
			existingSphere.expirationTime = Time.realtimeSinceStartup + duration;
		}
		else
		{
			// Add a new sphere if none exists at this position
			knownCellsSpheres.Add(new DebugSphere
			{
				position = position,
				color = color,
				expirationTime = Time.realtimeSinceStartup + duration
			});
		}

		// Force the editor to repaint so the sphere is drawn immediately
		SceneView.RepaintAll();
	}

	private void OnDrawGizmos()
	{
		// Draw all debug spheres
		for (int i = knownCellsSpheres.Count - 1; i >= 0; i--)
		{
			var sphere = knownCellsSpheres[i];
			if (Time.realtimeSinceStartup > sphere.expirationTime)
			{
				knownCellsSpheres.RemoveAt(i);
			}
			else
			{
				Handles.color = sphere.color;
				Handles.SphereHandleCap(0, sphere.position, Quaternion.identity, 0.5f, EventType.Repaint);
			}
		}
	}
	private List<Vector3> debugPath;
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
	//#endif
}