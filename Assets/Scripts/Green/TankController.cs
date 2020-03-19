using System;
using System.Collections.Generic;
using GreenStateMachine;
using UnityEngine;

namespace Green
{
    public enum TankState
    {
        PATROLLING
    }

    public class TankController : MonoBehaviour
    {
        public GameObject Bullet;
        private int health;
        private float elapsedTime;
        private float shootRate;
        private Transform turret;
        private Transform bulletSpawnPoint;
        private StateMachine<TankState> stateMachine;
        private IPlatoonController platoonController;

        // Solution or now to set the initial state
        private bool IsFirstStateSet = false;

        //Initialize the Finite state machine for the NPC tank
        protected void Awake()
        {
            health = 100;
            elapsedTime = 0.0f;
            shootRate = 2.0f;

            //Get the target enemy(Player)

            //Get the turret of the tank
            turret = gameObject.transform.GetChild(0).transform;
            bulletSpawnPoint = turret.GetChild(0).transform;

            //platoonController = GameObject.FindWithTag("PlatoonController").GetComponent<IPlatoonController>();

            var stateMap = new Dictionary<TankState, State<TankState>>
            {
                {TankState.PATROLLING, new PatrolState(gameObject, this)}
            };

            stateMachine = new StateMachine<TankState>(stateMap);
            
        }

        protected void Update()
        {
            // Solution or now to set the initial state
            if (!IsFirstStateSet) {
                stateMachine.transition(TankState.PATROLLING);
                IsFirstStateSet = false;
            }

            stateMachine.transition(stateMachine.act());
        }
    }
}