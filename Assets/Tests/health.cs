using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class health
{
    [Test]
    public void can_take_damage()
    {
        //SETUP - make a new Health component, setting current and max health
        //Health _health = new Health(20, 20);
        //ACT - deal 10 damage to it
        //_health.TakeDamage(10);
        //ASSERT - remaining health should be 10, 20-10=10
        //Assert.AreEqual(10, _health.CurrentHealth);
        Assert.Fail();
    }
}
