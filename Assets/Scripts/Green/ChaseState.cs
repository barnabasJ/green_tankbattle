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
        public ChaseState(GameObject gameObject, TankController tankcontroller) : base(gameObject)
        {
            this.tankController = tankController;
            this.targetEnemy = tankController.CurrentChaseEnemy();
        }

        public override void onStateEnter()
        {
            
        }
        public override void onStateExit()
        {
            
        }

        public override TankState? act(){
            return null;
        }
}
