using System;
using UnityEngine;

[Serializable]
public class ItemMemory
{
	[SerializeField] private string objectType;
	public string ObjectType { get => objectType; set => objectType = value; }

	private GameObject gameObject;
	public GameObject GameObject { get => gameObject; set => gameObject = value; }

	private Vector3 lastKnownLocation;
	public Vector3 LastKnownLocation { get => lastKnownLocation; set => lastKnownLocation = value; }

	[SerializeField] private bool itemIsUsable;
	public bool ItemIsUsable { get => itemIsUsable; set => itemIsUsable = value; }

	public ItemMemory(string objectType, GameObject gameObject, Vector3 lastKnownLocation, bool itemIsUsable)
	{
		this.objectType = objectType;
		this.gameObject = gameObject;
		this.lastKnownLocation = lastKnownLocation;
		this.itemIsUsable = itemIsUsable;
	}
}