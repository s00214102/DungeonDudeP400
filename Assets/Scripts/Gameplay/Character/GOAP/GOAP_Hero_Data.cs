using UnityEngine;

public class GOAP_Hero_Data : MonoBehaviour
{
	public HeroData HeroData;
	public HeroTraits HeroTraits;

	private void Awake()
	{
		GetComponent<Health>().SetMaxHealth(HeroData.HitPoints);
	}
	public void AssignTrait(HeroTraits newTraits)
	{
		HeroTraits = newTraits;
	}
}