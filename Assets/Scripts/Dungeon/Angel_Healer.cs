using UnityEngine;

// healing station the heroes can go to pray and regain their HP
// has a limited number of charges
public class Angel_Healer : MonoBehaviour
{
	[SerializeField] private int maxCharges = 0;
	private int chargesLeft = 0;

	private void Start()
	{
		chargesLeft = maxCharges;
	}
	public void UseCharge()
	{
		if (CanHeal())
			chargesLeft--;
		if (chargesLeft <= 0)
			Destroy(this.gameObject); // replace this later
	}
	public bool CanHeal()
	{
		if (chargesLeft > 0)
			return true;
		else
			return false;
	}
}