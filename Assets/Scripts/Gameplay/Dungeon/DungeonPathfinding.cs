using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonPathfinding
{
	private static Vector3Int[] Directions = { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right };
	private static HashSet<Vector3Int> visitedNodes = new HashSet<Vector3Int>();
	private static int maxIterations = 1000;

	public static List<Vector3Int> BreadthFirstSearch(DungeonCell[,] grid, Vector3Int start, Vector3Int target)
	{
		Queue<Vector3Int> queue = new Queue<Vector3Int>();
		Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

		queue.Enqueue(start);
		cameFrom[start] = start;

		while (queue.Count > 0)
		{
			Vector3Int current = queue.Dequeue();

			if (current == target)
			{
				// Reconstruct path
				List<Vector3Int> path = new List<Vector3Int>();
				while (current != start)
				{
					path.Add(current);
					current = cameFrom[current];
				}
				path.Reverse(); // Reverse to get correct order
				return path;
			}

			// Explore neighbors
			foreach (Vector3Int direction in Directions)
			{
				Vector3Int neighbor = current + direction;

				// Check if neighbor is within grid bounds and is walkable
				if (IsValidPosition(grid, neighbor) && !cameFrom.ContainsKey(neighbor))
				{
					queue.Enqueue(neighbor);
					cameFrom[neighbor] = current;
				}
			}
		}

		// If target is unreachable, return empty path
		return new List<Vector3Int>();
	}

	public static List<Vector3> BreadthFirstSearch2(DungeonCell[,] grid, Vector3Int start, Vector3Int goal)
	{
		Queue<Vector3Int> queue = new Queue<Vector3Int>();
		Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

		queue.Enqueue(start);
		cameFrom[start] = start;

		while (queue.Count > 0)
		{
			Vector3Int current = queue.Dequeue();

			if (current == goal)
			{
				// Reconstruct path
				List<Vector3> path = new List<Vector3>();
				while (current != start)
				{
					path.Add(current);
					current = cameFrom[current];
				}
				path.Reverse(); // Reverse to get correct order
				return path;
			}

			// Explore neighbors
			foreach (Vector3Int direction in Directions)
			{
				Vector3Int neighbor = current + direction;

				// Check if neighbor is within grid bounds and is walkable
				if (IsValidPosition(grid, neighbor) && !cameFrom.ContainsKey(neighbor))
				{
					queue.Enqueue(neighbor);
					cameFrom[neighbor] = current;
				}
			}
		}

		// If target is unreachable, return empty path
		return new List<Vector3>();
	}

	// uses breadth first search with a priority queue based on heuristic cost and returns the first path to the goal it finds
	public static List<Vector3> BestCostFirstSearch(DungeonCell[,] grid, Vector3Int start, Vector3Int goal)
	{
		PriorityQueue<Vector3Int> openSet = new PriorityQueue<Vector3Int>();
		Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

		openSet.Enqueue(start, 0);
		cameFrom[start] = start;
		visitedNodes.Clear();

		int iterations = 0;
		while (openSet.Count > 0 && iterations < maxIterations)
		{
			Vector3Int current = openSet.Dequeue();
			visitedNodes.Add(current);
			//DebugDrawCell(grid, current, Color.green);

			if (current == goal)
			{
				List<Vector3> path = ReconstructPath(cameFrom, start, current);
				cameFrom.Clear();
				visitedNodes.Clear();
				return path;
			}

			foreach (Vector3Int direction in Directions)
			{
				Vector3Int neighbor = current + direction;

				if (IsValidPosition(grid, neighbor) && !cameFrom.ContainsKey(neighbor) && !visitedNodes.Contains(neighbor))
				{
					openSet.Enqueue(neighbor, GetHeuristicCost(neighbor, goal));
					cameFrom[neighbor] = current;
				}
			}
			iterations++;
		}
		visitedNodes.Clear(); // Clear visited nodes set
		return new List<Vector3>();
	}

	// builds on best cost search but also uses the movement cost of a cell to order the queue, finds all paths to the goal as returns the best one
	public static List<Vector3> AStarSearch(DungeonCell[,] grid, Vector3Int start, Vector3Int goal)
	{
		PriorityQueue<Vector3Int> openSet = new PriorityQueue<Vector3Int>();
		Dictionary<Vector3Int, int> gCost = new Dictionary<Vector3Int, int>();
		Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

		openSet.Enqueue(start, 0);
		gCost[start] = 0;
		cameFrom[start] = start;

		List<Vector3> lowestCostPath = null;
		int lowestCost = int.MaxValue;

		while (openSet.Count > 0)
		{
			Vector3Int current = openSet.Dequeue();
			//System.Diagnostics.Debug.WriteLine("Current position: " + current.ToString());

			if (current == goal)
			{
				int pathCost = gCost[current];
				System.Diagnostics.Debug.WriteLine("Goal found! cost: " + gCost[current]);
				if (pathCost < lowestCost)
				{
					lowestCost = pathCost;
					lowestCostPath = ReconstructPath(cameFrom, start, current);
					DebugWritePath(lowestCostPath);
				}
				continue;
			}

			// Explore neighbors
			foreach (Vector3Int direction in Directions)
			{
				Vector3Int neighbor = current + direction;

				// Check if neighbor is and valid and not a node we came from
				if (IsValidPosition(grid, neighbor) && !cameFrom.ContainsKey(neighbor))
				{
					int newCost = gCost[current] + GetGCost(current, neighbor, grid);

					gCost[neighbor] = newCost;
					int priority = newCost + GetGCost(neighbor, goal, grid);
					openSet.Enqueue(neighbor, priority);
					cameFrom[neighbor] = current;

				}
			}
		}

		return lowestCostPath;
	}

	private static List<Vector3> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int start, Vector3Int end)
	{
		List<Vector3> path = new List<Vector3>();
		Vector3Int current = end;

		while (current != start)
		{
			path.Add(current);
			current = cameFrom[current];
		}
		cameFrom.Clear();
		path.Reverse();
		return path;
	}

	// Method to check if a position is valid (within grid bounds and walkable)
	private static bool IsValidPosition(DungeonCell[,] grid, Vector3Int position)
	{
		int rows = grid.GetLength(0);
		int cols = grid.GetLength(1);
		return position.x >= 0 && position.x < rows &&
			   position.z >= 0 && position.z < cols &&
			   grid[position.x, position.z].isWalkable; // Modify condition based on your grid setup
	}

	private static int GetHeuristicCost(Vector3Int from, Vector3Int to)
	{
		// Assuming uniform cost for now (Manhattan distance)
		return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.z - to.z);
	}

	private static int GetGCost(Vector3Int from, Vector3Int to, DungeonCell[,] grid)
	{
		DungeonCell fromCell = grid[from.x, from.z];
		DungeonCell toCell = grid[to.x, to.z];

		// Assuming uniform cost for now (Manhattan distance)
		int heuristicCost = GetHeuristicCost(from, to);

		// Add the movement cost of the destination cell to the heuristic cost
		heuristicCost += toCell.movementCost;

		return heuristicCost;
	}

	private static void DebugDrawCell(DungeonCell[,] grid, Vector3Int position, Color color)
	{
		DungeonCell cell = grid[position.x, position.z];
		Vector3 cellCenter = cell.worldPositionCenter;
		//float halfCellSize = 0.5f;

		// Draw a cube at the cell position
		Gizmos.color = color;
		Gizmos.DrawCube(cellCenter, new Vector3(1, 1, 1));
	}

	private static void DebugWritePath(List<Vector3> path)
	{
		if (path == null || path.Count == 0)
		{
			System.Diagnostics.Debug.WriteLine("No positions provided.");
			return;
		}

		string output = "Path: ";
		foreach (var position in path)
		{
			output += position + " ";
		}
		System.Diagnostics.Debug.WriteLine(output);
	}
}
