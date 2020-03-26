using System.Collections;
using System.Collections.Generic;
using GreenStateMachine;
using UnityEngine;
using Green;

public class ChaseState : State<TankState>
{
    private TankController tankController;
    private Target target;
    private Collider targetEnemy;
        public ChaseState(GameObject gameObject, TankController tankcontroller) : base(gameObject)
        {
            target = new Target();
            this.tankController = tankController;
        }

        // when enter this state, get the current enemy that the group is chasing
        public override void onStateEnter()
        {
            List<Collider> enemiesSpotted = tankController.EnemiesInAttackRange();
            if ( enemiesSpotted.Count > 0)
            {
                this.targetEnemy = enemiesSpotted[0];
            }
            
        }
        public override void onStateExit()
        {
            
        }

        public override TankState? act(){
            target.UpdateTargets(targetEnemy.transform.position);
            return null;
        }
}
