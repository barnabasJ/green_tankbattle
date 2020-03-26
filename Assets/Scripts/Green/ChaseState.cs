using System.Collections;
using System.Collections.Generic;
using GreenStateMachine;
using UnityEngine;
using Green;

public class ChaseState : State<TankState>
{
    private TankController tankController;
    private Target target;
    private GameObject targetEnemy;
        public ChaseState(GameObject gameObject, TankController tankCtrl) : base(gameObject)
        {
            target = Object.FindObjectOfType<Target>();
            this.tankController = tankCtrl;
        }

        // when enter this state, get the current enemy that the group is chasing
        public override void onStateEnter()
        {
            this.targetEnemy = tankController.CurrentChaseEnemy();
        }
        public override void onStateExit()
        {
            
        }

        public override TankState? act(){
            target.UpdateTargets(targetEnemy.transform.position);
            return null;
        }
}
