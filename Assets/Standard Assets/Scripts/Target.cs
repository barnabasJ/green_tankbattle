using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent[] navAgents;
    public Transform targetMarker;

    public void Start ()
    {
	    navAgents = FindObjectsOfType(typeof(UnityEngine.AI.NavMeshAgent)) as UnityEngine.AI.NavMeshAgent[];
    }

    public void UpdateTargets ( Vector3 targetPosition  )
    {
	    foreach(UnityEngine.AI.NavMeshAgent agent in navAgents) 
        {
		    agent.destination = targetPosition;
        }
    }
}