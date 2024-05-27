using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T>
{
    public T owner;

    public State(T owner)
    {
        this.owner = owner;
    }

    public abstract bool Check(float deltaTime);
    public abstract void Enter();
    public abstract void Exit();
}
