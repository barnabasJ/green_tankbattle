using System.Collections.Generic;
using Green;
using GreenStateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Green
{
    public class PatrolState : State<TankState>
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
            _wayPoints = Object.FindObjectOfType<PatrollingWayPoints>().GetComponent<PatrollingWayPoints>().wayPoints;
            _currentWayPointIndex = 0;
        }


        public override void onStateEnter()
        {
            _tankController.Start();
        }

        public override void onStateExit()
        {
            _tankController.Stop();
        }

        public override TankState? act()
        {
            if (_tankController.platoonController.getEnemyTarget() != null) return TankState.CHASE;

            if (PlatoonHasReachedItsDestination())
                _currentWayPointIndex = (_currentWayPointIndex + 1) % _wayPoints.Length;
            _tankController.GetComponent<NavMeshAgent>().destination = _wayPoints[_currentWayPointIndex];
            return null;
        }


        private void OrderToPatrolTowardsNextWayPoint()
        {
        }

        private bool PlatoonHasReachedItsDestination()
        {
            // Here we check if all tanks in the platoon are close enough towards the current waypoint.
            // If one tank is not close enough this function will return false.  
            return Vector3.Distance(gameObject.transform.position, _wayPoints[_currentWayPointIndex]) <=
                   gameObject.GetComponent<NavMeshAgent>().stoppingDistance + 1;
        }
    }
}