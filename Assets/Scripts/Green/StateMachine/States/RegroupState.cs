using System.Collections.Generic;
using GreenStateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Green
{
    public class RegroupState: State<TankState>
    {

        private readonly Vector3 regroupingLocation;

        private readonly Target target;

        private readonly TankController _tankController;
        
        public RegroupState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            target = new Target();
            _tankController = tankController;
            regroupingLocation = _tankController.platoonController.getPlatoonMeanPosition();
            OrderTanksTowardsRegroupingLocation();
        }

        public override TankState? act()
        {
            if (PlatoonHasRegrouped()) return TankState.PATROLLING;
            return null;
        }

        private bool PlatoonHasRegrouped()
        {
            List<GameObject> tanksInPlatoon = _tankController.platoonController.getAliveTanks();
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