using System;
using UnityEngine;

public struct HeroMemory
{
	private string objectType;
	public string ObjectType { get => objectType; set => objectType = value; }

	private GameObject gameObject;
	public GameObject GameObject { get => gameObject; set => gameObject = value; }

	private Vector3 lastKnownLocation;
	public Vector3 LastKnownLocation { get => lastKnownLocation; set => lastKnownLocation = value; }

	public HeroMemory(string objectType, GameObject gameObject, Vector3 lastKnownLocation)
	{
		this.objectType = objectType;
		this.gameObject = gameObject;
		this.lastKnownLocation = lastKnownLocation;
	}
}