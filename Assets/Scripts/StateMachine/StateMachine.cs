using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class StateMachine<T> where T : struct, IConvertible
    {
        private State<T> currentState;
        private Dictionary<T, State<T>> stateMap;
        private T keepState;

        public StateMachine(Dictionary<T, State<T>> stateMap, T keepState)
        {
            this.stateMap = stateMap;
            this.keepState = keepState;
        }

        public T act()
        {
            return currentState.act();
        }

        public void transition(T e)
        {
            if (e.Equals(keepState))
                return;
            if (stateMap.ContainsKey(e))
            {
                currentState?.onStateExit();
                Debug.Log("Exiting: " + currentState);
                currentState = stateMap[e];
                currentState.onStateEnter();
                Debug.Log("Entering: " + currentState);
            }
            else
                throw new ArgumentException("There is no tranisiton for this event");
        }
    }
}