using System;
using UnityEngine;

public struct ItemMemory
{
	private string objectType;
	public string ObjectType { get => objectType; set => objectType = value; }

	private GameObject gameObject;
	public GameObject GameObject { get => gameObject; set => gameObject = value; }

	private Vector3 lastKnownLocation;
	public Vector3 LastKnownLocation { get => lastKnownLocation; set => lastKnownLocation = value; }

	public bool itemIsUsable;

	public ItemMemory(string objectType, GameObject gameObject, Vector3 lastKnownLocation, bool itemIsUsable)
	{
		this.objectType = objectType;
		this.gameObject = gameObject;
		this.lastKnownLocation = lastKnownLocation;
		this.itemIsUsable = itemIsUsable;
	}
}