using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Death_State : BaseState
{
    HeroController controller;
    Transform target;
    public Hero_Death_State(HeroController controller) : base(controller)
    {
        this.controller = controller;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {
        //controller.CharacterMovement.DestinationReached.RemoveListener(OnCoverReached);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
