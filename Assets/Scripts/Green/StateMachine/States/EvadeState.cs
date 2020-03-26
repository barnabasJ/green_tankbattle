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
        private float evadeRadius = 25f;

        private Vector3 evadeDir;

        public EvadeState(GameObject givenGameObject, TankController givenTankController) : base(givenGameObject)
        {
            tankController = givenTankController;
        }

        public override TankState? act()
        {
            var tanks = tankController.tanksInCrashDistance(evadeRadius);

            if (tanks.Count <= 0) return TankState.REGROUPING;

            foreach (var tank in tanks)
            {
                // Evades friendly tanks.
                if (tank.transform.GetComponent<NavMeshAgent>() != null)
                {
                    Vector3 dir = (tank.transform.position - gameObject.transform.position).normalized;

                    if (dir.z >= 0)
                        continue;

                    // The weight is inversely related to how close this tank is the other tank.
                    float weight = Mathf.InverseLerp(0f, evadeRadius,
                        (tank.transform.position - gameObject.transform.position).magnitude);
                    evadeDir += new Vector3(dir.x * -1f, dir.y, dir.z) * weight;
                }
            }

            evadeDir.Normalize();
            tankController.GetComponent<NavMeshAgent>().destination = evadeDir * 10f;
            return null;
        }
    }
}