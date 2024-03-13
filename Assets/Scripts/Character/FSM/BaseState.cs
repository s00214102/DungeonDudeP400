using System.Collections;
using System.Collections.Generic;

public abstract class BaseState
{
    public BaseState(BaseStateMachine controller) { }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
