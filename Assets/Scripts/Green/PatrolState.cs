using System.Collections.Generic;
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
        private readonly List<Vector3> _wayPoints;
        
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
            _wayPoints = new List<Vector3> {
                new Vector3(930, 113.3f, 1945), 
                new Vector3(971, 113.3f, 2272), 
                new Vector3(1202, 113.3f, 2243)
            };
            _currentWayPointIndex = 0;
        }

        public override void onStateEnter()
        {
            target.UpdateTargets(_wayPoints[_currentWayPointIndex]);
        }

        public override TankState? act()
        {
            if(PlatoonHasReachedItsDestination()) OrderToPatrolTowardsNextWayPoint();
            
            //todo Spot enemy's
            
            
            
            return null;
        }

        private void OrderToPatrolTowardsNextWayPoint()
        {
            if (_currentWayPointIndex >= _wayPoints.Count - 1) {
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