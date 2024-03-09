using System;
using UnityEngine;

public struct TreasureMemory
{
	private Treasure treasure;
	public Treasure Treasure { get => treasure; set => treasure = value; }

	private Vector3 lastKnownLocation;
	public Vector3 LastKnownLocation { get => lastKnownLocation; set => lastKnownLocation = value; }

	public TreasureMemory(Treasure treasure, Vector3 lastKnownLocation)
	{
		this.treasure = treasure;
		this.lastKnownLocation = lastKnownLocation;
	}
}