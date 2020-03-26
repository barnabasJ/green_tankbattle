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
    public List<Collider> spottedEnemies;
        public ChaseState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            this.tankController = tankController;
            target = new Target();
        }

        // when enter this state, get the current enemy that the group is chasing
        public override void onStateEnter()
        {
            spottedEnemies = tankController.EnemiesInAttackRange();
            if ( spottedEnemies.Count > 0)
            {
                this.targetEnemy = spottedEnemies[0];
            }
            
        }
        public override void onStateExit()
        {
            
        }

        public override TankState? act(){
            if(spottedEnemies.Count > 0) {
                target.UpdateTargets(targetEnemy.gameObject.transform.position);
            }
            return null;
        }
}
