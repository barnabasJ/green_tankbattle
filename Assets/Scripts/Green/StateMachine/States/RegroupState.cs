using System.Collections.Generic;
using GreenStateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Green
{
    public class RegroupState : State<TankState>
    {
        private readonly Vector3 regroupingLocation;

        private readonly Target target;

        private readonly TankController _tankController;

        public RegroupState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            _tankController = tankController;
        }

        public override TankState? act()
        {
            if (PlatoonHasRegrouped()) return TankState.PATROLLING;

            _tankController.GetComponent<NavMeshAgent>().destination =
                _tankController.platoonController.getPlatoonMeanPosition();
            return null;
        }

        private bool PlatoonHasRegrouped()
        {
            return Vector3.Distance(_tankController.transform.position,
                _tankController.platoonController.getPlatoonMeanPosition()) < 100f;
        }

        private void OrderTanksTowardsRegroupingLocation()
        {
            target.UpdateTargets(regroupingLocation);
        }
    }
}