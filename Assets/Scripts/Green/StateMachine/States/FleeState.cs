using System.Collections;
using System.Collections.Generic;
using GreenStateMachine;
using UnityEngine;
using Green;
using UnityEngine.AI;

public class FleeState : State<TankState>
{
    // flee state starts when enemy number overpowers platoon number
    private TankController _tankController;
    
    private readonly Vector3[] _wayPoints;
    
    public List<Collider> spottedEnemies;
        public FleeState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            _wayPoints = Object.FindObjectOfType<PatrollingWayPoints>().GetComponent<PatrollingWayPoints>().wayPoints;
            _tankController = tankController;

        }

        public override void onStateEnter()
        {
            _tankController.Start();
        }

        public override void onStateExit()
        {
            _tankController.Stop();
        }

        public override TankState? act(){

            spottedEnemies = _tankController.SpottedEnemies();
            //if no enemy left in sight, regroup
            if(_tankController.platoonController.getEnemyTarget() == null) {
                return TankState.REGROUPING;

            // flee if enemy number outnumber us
            } else if(spottedEnemies.Count > _tankController.platoonController.getAliveTanks().Count) {
                //Debug.Log("flee");
                Flee();

            // return to chasing if enemy number drops
            } else if(spottedEnemies.Count <= _tankController.platoonController.getAliveTanks().Count) {
                return TankState.CHASE;
            }
            return null;
        }

        // return a flee position for the platoon
        private void Flee() {
            var fleePosition = _tankController.EnemyMeanPosition()*-1 + 2*_tankController.platoonController.getPlatoonMeanPosition();
            Vector3 farthestWP = _wayPoints[0];
            foreach (Vector3 wp in _wayPoints) {
                if (Vector3.Distance(wp,_tankController.EnemyMeanPosition()) > Vector3.Distance(farthestWP,_tankController.EnemyMeanPosition())){
                    farthestWP = wp;
                }
            }
            
            _tankController.GetComponent<NavMeshAgent>().destination = farthestWP;
    
        }
}

