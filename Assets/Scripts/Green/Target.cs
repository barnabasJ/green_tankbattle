using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Green
{
    public class Target
    {
        public void UpdateTargets ( Vector3 targetPosition  )
        {
            List<GameObject> tanks = new List<GameObject>(GameObject.FindGameObjectsWithTag("GreenTank"));
            foreach (var tank in tanks) {
                tank.GetComponent<NavMeshAgent>().destination = targetPosition;
            }
        }
    }
}