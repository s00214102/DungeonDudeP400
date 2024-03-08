using UnityEngine;

public class GOAP_Hero_Data : MonoBehaviour
{
	public HeroData HeroData;

	[SerializeField] private Transform goalTransform;
	[HideInInspector] public Vector3 goal => goalTransform.position;
}