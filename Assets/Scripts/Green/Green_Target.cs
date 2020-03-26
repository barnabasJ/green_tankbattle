using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// target class that only grabs green tanks
public class Green_Target : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent[] navAgents;
    public List<UnityEngine.AI.NavMeshAgent> greenNavAgents = new List<UnityEngine.AI.NavMeshAgent>();
    public Transform targetMarker;

    void Start ()
    {
	    navAgents = FindObjectsOfType(typeof(UnityEngine.AI.NavMeshAgent)) as UnityEngine.AI.NavMeshAgent[];
        foreach(UnityEngine.AI.NavMeshAgent agent in navAgents) {
            if(agent.gameObject.CompareTag("green-tank")){
                greenNavAgents.Add(agent);
            }
        }
		UpdateTargets (targetMarker.position);
    }

    public void UpdateTargets ( Vector3 targetPosition  )
    {
	    foreach(UnityEngine.AI.NavMeshAgent agent in greenNavAgents) 
        {
		    agent.destination = targetPosition;
	    }
    }
}
