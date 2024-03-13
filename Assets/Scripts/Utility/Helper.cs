using UnityEngine;

static public class Helper
{
	/// <summary>
	/// Pass two positions and a range. Returns true if the distance between them is less than the range.
	/// </summary>
	/// <param name="pos1"></param>
	/// <param name="pos2"></param>
	/// <param name="range"></param>
	/// <returns></returns>
	public static bool InRange(Vector3 pos1, Vector3 pos2, float range)
	{
		float dist = Vector3.Distance(pos1, pos2);
		return dist <= range;
	}
}