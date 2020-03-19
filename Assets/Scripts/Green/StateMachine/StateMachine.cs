using System;
using System.Collections.Generic;
using Green;
using GreenStateMachine;
using UnityEngine;

namespace GreenStateMachine
{
    public class StateMachine<T> where T : struct, IConvertible
    {
        private State<T> currentState;
        private Dictionary<T, State<T>> stateMap;

        public StateMachine(Dictionary<T, State<T>> stateMap)
        {
            this.stateMap = stateMap;
        }

        public T? act()
        {
            return currentState.act();
        }

        public void transition(T? e)
        {
            if (e == null)
                return;
            if (stateMap.ContainsKey((T) e))
            {
                currentState?.onStateExit();
                Debug.Log("Exiting: " + currentState);
                currentState = stateMap[(T) e];
                currentState.onStateEnter();
                Debug.Log("Entering: " + currentState);
            }
            else
                throw new ArgumentException("There is no tranisiton for this event");
        }
    }
}