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
            evadeDir = gameObject.transform.forward;

            // Looks for objects around this tank.
            hitInfo = Physics.SphereCastAll(gameObject.transform.position, evadeRadius, gameObject.transform.forward);

            foreach (RaycastHit hit in hitInfo)
            {
                // Evades friendly tanks.
                if (hit.transform.GetComponent<NavMeshAgent>() != null)
                {
                    Vector3 dir = (hit.transform.position - gameObject.transform.position).normalized;

                    if (dir.z >= 0)
                        continue;

                    // The weight is inversely related to how close this tank is the other tank.
                    float weight = Mathf.InverseLerp(0f, evadeRadius, (hit.transform.position - gameObject.transform.position).magnitude);
                    evadeDir += new Vector3(dir.x * -1f, dir.y, dir.z) * weight;
                }
            }

            evadeDir.Normalize();


            // Should go into regroup or flee.
            return TankState.PATROLLING;
        }
    }
}