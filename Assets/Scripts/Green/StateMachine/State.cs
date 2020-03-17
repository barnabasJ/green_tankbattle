using System;
using UnityEngine;

namespace GreenStateMachine
{
    public abstract class State<T> where T : struct, IConvertible
    {
        protected GameObject gameObject;

        public State(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public virtual void onStateEnter()
        {
        }

        public abstract T? act();

        public virtual void onStateExit()
        {
        }
    }
}