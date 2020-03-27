using System.Collections;
using System.Collections.Generic;
using GreenStateMachine;
using UnityEngine;
using Green;
using UnityEngine.AI;

namespace Green
{
    public class ChaseState : State<TankState>
    {
        private TankController tankController;

        public ChaseState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            this.tankController = tankController;
        }
        
        public override void onStateEnter()
        {
            tankController.Start();
        }

        public override void onStateExit()
        {
            tankController.Stop();
        }

        public override TankState? act()
        {
            var target = tankController.platoonController.getEnemyTarget();

            // no targets -> regroup
            if (target == null)
                return TankState.REGROUPING;

            // enemies in attackRange -> attack 
            if (tankController.EnemiesInAttackRange().Count > 0)
                return TankState.ATTACKING;

            // calculate the nearest position in the middle of the attack range
            var directionFromEnemyToPlayer = (tankController.transform.position -
                                              target.transform.position)
                .normalized;

            var dest = target.transform.position + directionFromEnemyToPlayer *
                (tankController.maxAttackRange - tankController.minAttackRange);

            // move towards it 
            tankController.GetComponent<NavMeshAgent>().destination = dest;
            return null;
        }
    }
}