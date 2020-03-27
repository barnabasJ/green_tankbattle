using System.Collections;
using System.Collections.Generic;
using GreenStateMachine;
using UnityEngine;
using Green;

public class FleeState : State<TankState>
{
    // flee state starts when enemy number overpowers platoon number
    private TankController tankController;
    private Target target;
    public List<Collider> spottedEnemies;
        public FleeState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            this.tankController = tankController;
            target = new Target();
        }

        
        public override void onStateEnter()
        {
            
        }
        public override void onStateExit()
        {
            
        }

        public override TankState? act(){
            var target = tankController.platoonController.getEnemyTarget();

            //if no enemy left in sight, regroup
            if(target == null) {
                return TankState.REGROUPING;

            // flee if enemy number outnumber us
            } else if(spottedEnemies.Count > tankController.platoonController.getAliveTanks().Count) {
                Flee();

            // return to chasing if enemy number drops
            } else if(spottedEnemies.Count < tankController.platoonController.getAliveTanks().Count) {
                return TankState.CHASE;
            }
            return null;
        }

        // return a flee position for the platoon
        private void Flee() {
            Vector3 fleePosition = tankController.platoonController.getPlatoonMeanPosition() * 2 - tankController.EnemyMeanPosition();
            target.UpdateTargets(fleePosition);
        }
}

