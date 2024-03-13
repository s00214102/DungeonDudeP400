using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class helper
{
	[Test]
	public void in_range_true()
	{
		// SETUP
		Vector3 pos1 = new Vector3(0, 5, 0);
		Vector3 pos2 = new Vector3(0, 1, 0);
		float range = 10;
		// ACT 
		Helper.InRange(pos1, pos2, range);
		// ASSERT 
		Assert.IsTrue(Helper.InRange(pos1, pos2, range));
	}
	[Test]
	public void in_range_false()
	{
		// SETUP
		Vector3 pos1 = new Vector3(0, 25, 0);
		Vector3 pos2 = new Vector3(0, 1, 0);
		float range = 10;
		// ACT 
		Helper.InRange(pos1, pos2, range);
		// ASSERT 
		Assert.IsFalse(Helper.InRange(pos1, pos2, range));
	}
}