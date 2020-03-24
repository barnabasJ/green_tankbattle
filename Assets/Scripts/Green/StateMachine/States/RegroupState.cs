using GreenStateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Green
{
    public class RegroupState: State<TankState>
    {

        private readonly Vector3 regroupingLocation;

        private readonly Target target;
        
        public RegroupState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            target = new Target();
            
            regroupingLocation = CalculateRegroupingLocation();
            OrderTanksTowardsRegroupingLocation();
        }

        public override TankState? act()
        {
            if (PlatoonHasRegrouped()) return TankState.PATROLLING;
            return null;
        }

        private Vector3 CalculateRegroupingLocation()
        {
            TankController[] tanksInPlatoon = Object.FindObjectsOfType<TankController>();
            
            Vector3 tankLocationSum = new Vector3(0,0,0);
            foreach (var tank in tanksInPlatoon) { tankLocationSum += tank.gameObject.transform.position; }

            Vector3 averagelocation = tankLocationSum / tanksInPlatoon.Length;
            return averagelocation;
        }

        private bool PlatoonHasRegrouped()
        {
            TankController[] tanksInPlatoon = Object.FindObjectsOfType<TankController>();
            foreach (var tank in tanksInPlatoon)
            {
                NavMeshAgent navMeshAgent = tank.gameObject.GetComponent<NavMeshAgent>();
                float distanceTowardsRegroupingLocation = Vector3.Distance(tank.gameObject.transform.position, 
                    regroupingLocation); 
                if (distanceTowardsRegroupingLocation > navMeshAgent.stoppingDistance) return false;
            }
            return true;
        }

        private void OrderTanksTowardsRegroupingLocation()
        {
            target.UpdateTargets(regroupingLocation);
        }
    }
}