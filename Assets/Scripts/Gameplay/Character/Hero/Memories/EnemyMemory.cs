using System;
using UnityEngine;

[Serializable]
public class EnemyMemory
{
	[SerializeField] private string objectType;
	public string ObjectType { get => objectType; set => objectType = value; }

	private GameObject gameObject;
	public GameObject GameObject { get => gameObject; set => gameObject = value; }

	[SerializeField] private Vector3 lastKnownLocation;
	public Vector3 LastKnownLocation { get => lastKnownLocation; set => lastKnownLocation = value; }

	// alert level for this enemy
	[SerializeField] private float alertLevel;
	public float AlertLevel {get=>alertLevel; set => alertLevel = value;}

	public EnemyMemory(string objectType, GameObject gameObject, Vector3 lastKnownLocation, float alertLevel)
	{
		this.objectType = objectType;
		this.gameObject = gameObject;
		this.lastKnownLocation = lastKnownLocation;
		this.alertLevel = alertLevel;
	}
}