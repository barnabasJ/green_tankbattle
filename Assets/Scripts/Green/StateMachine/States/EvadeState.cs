using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GreenStateMachine;

namespace Green
{
    public class EvadeState : State<TankState>
    {
        private TankController tankController;

        private RaycastHit[] hitInfo;
        public static float evadeRadius;

        public static TankState previousState;

        public EvadeState(GameObject givenGameObject, TankController givenTankController) : base(givenGameObject)
        {
            tankController = givenTankController;
            evadeRadius = tankController.evadeDistance;
        }

        public override void onStateEnter()
        {
            tankController.Start();
        }

        public override void onStateExit()
        {
            tankController.Stop();
        }

        public override TankState? act()
        {
            tankController.Aim();

            var tanks = tankController.tanksInCrashDistance(evadeRadius);

            if (tanks.Count <= 0) return previousState;

            var evadeDir = Vector3.zero;
            foreach (var tank in tanks)
            {
                Vector3 dir = (gameObject.transform.position - tank.transform.position).normalized;

                // The weight is inversely related to how close this tank is the other tank.
                float weight = Mathf.InverseLerp(0f, evadeRadius,
                    (tank.transform.position - gameObject.transform.position).magnitude);
                evadeDir += dir * weight;
            }

            evadeDir.Normalize();
            tankController.GetComponent<NavMeshAgent>().destination =
                gameObject.transform.position +
                evadeDir * (evadeRadius + tankController.GetComponent<NavMeshAgent>().stoppingDistance + 10);
            return null;
        }
    }
}