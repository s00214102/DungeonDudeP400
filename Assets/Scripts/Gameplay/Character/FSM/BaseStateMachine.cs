using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseStateMachine : MonoBehaviour
{
    public BaseState CurrentImplimentation;
    public UnityEvent<int> StateChanged;

    public virtual void SetState(int newState)
    {
        StateChanged?.Invoke(newState);
    }
}
