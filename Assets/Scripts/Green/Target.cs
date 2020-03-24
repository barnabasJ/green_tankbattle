using UnityEngine;
using UnityEngine.AI;

namespace Green
{
    public class Target
    {
        public void UpdateTargets ( Vector3 targetPosition  )
        {
            if (!(Object.FindObjectsOfType(typeof(NavMeshAgent)) is NavMeshAgent[] navAgents)) return;
            foreach (NavMeshAgent agent in navAgents) {
                agent.destination = targetPosition;
            }
        }
    }
}