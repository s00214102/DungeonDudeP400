using UnityEngine;

public class GOAP_Hero_Data : MonoBehaviour
{
	public HeroData HeroData;
	public HeroTraits HeroTraits;

	private void Awake()
	{
		GetComponent<Health>().SetMaxHealth(HeroData.HitPoints);
		GetComponent<CharacterMovement>().baseSpeed = HeroData.Speed;
	}
	public void AssignClass(HeroData data)
	{
		HeroData = data;
	}
	public void AssignTrait(HeroTraits newTraits)
	{
		HeroTraits = newTraits;
	}
}