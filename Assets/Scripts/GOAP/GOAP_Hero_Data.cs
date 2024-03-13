using UnityEngine;

public class GOAP_Hero_Data : MonoBehaviour
{
	public HeroData HeroData;
	public HeroTraits HeroTraits;

	[SerializeField] private Transform goalTransform;
	[HideInInspector] public Vector3 goal => goalTransform.position;
	public Transform Angel; // manually setting an angels location for testing

	private void Awake()
	{
		GetComponent<Health>().SetMaxHealth(HeroData.HitPoints);
	}
}