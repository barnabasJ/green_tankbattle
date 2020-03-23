using System.Collections.Generic;
using DefaultNamespace;
using GreenStateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Green
{
    public class PatrolState: State<TankState>
    {
        private TankController _tankController;
        
        // <summary>
        // This list contains the sequential locations the platoon should patrol towards.
        // If a point is reached, the platoon will patrol to the next location in this list.
        // If the last point is reached the platoon will patrol back to the first point.
        // </summary>
        private readonly Vector3[] _wayPoints;
        
        // <summary>
        // This is the index off the current waypoint in the _wayPoints list.
        // The tank platoon will move towards the current waypoint indicated with this index.
        // </summary>        
        private int _currentWayPointIndex;

        private readonly Target target;

        public PatrolState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            _tankController = tankController;
            target = Object.FindObjectOfType<Target>();
            _wayPoints = Object.FindObjectOfType<PatrollingWayPoints>().GetComponent<PatrollingWayPoints>().wayPoints;
            _currentWayPointIndex = 0;
        }

        public override void onStateEnter()
        {
            target.UpdateTargets(_wayPoints[_currentWayPointIndex]);
        }

        public override TankState? act()
        {
            if(PlatoonHasReachedItsDestination()) OrderToPatrolTowardsNextWayPoint();

            if (EnemiesAreInSight()) return TankState.CHASE;
            
            return null;
        }

        private bool EnemiesAreInSight()
        {
            return _tankController.platoonController.getCurrentEnemyCount() > 0;
        }

        private void OrderToPatrolTowardsNextWayPoint()
        {
            if (_currentWayPointIndex >= _wayPoints.Length - 1) {
                _currentWayPointIndex = 0;
            } else {
                _currentWayPointIndex++;
            }
            
            target.UpdateTargets(_wayPoints[_currentWayPointIndex]);
        }

        private bool PlatoonHasReachedItsDestination()
        {
            TankController[] tanksInPlatoon = Object.FindObjectsOfType<TankController>();
            
            // Here we check if all tanks in the platoon are close enough towards the current waypoint.
            // If one tank is not close enough this function will return false.  
            foreach (TankController tank in tanksInPlatoon) { 
               NavMeshAgent navMeshAgent = tank.gameObject.GetComponent<NavMeshAgent>();
               float distanceTowardsDestination = Vector3.Distance(tank.gameObject.transform.position, 
                   _wayPoints[_currentWayPointIndex]); 
               if (distanceTowardsDestination > navMeshAgent.stoppingDistance) return false;
            }
            return true;
        }
    }
}