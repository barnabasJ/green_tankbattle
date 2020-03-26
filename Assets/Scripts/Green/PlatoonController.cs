using System;
using System.Collections.Generic;
using UnityEngine;

namespace Green
{
    /// <summary>
    /// PlatoonController
    ///
    /// Houses all the information shared between all tanks
    /// </summary>
    public class PlatoonController : MonoBehaviour, IPlatoonController
    {
        private List<GameObject> aliveTanks = new List<GameObject>();
        private Vector3 platoonMeanPosition;
        private HashSet<GameObject> spottedEnemies = new HashSet<GameObject>();
        private List<GameObject> targets = new List<GameObject>();
        private List<GameObject> tanks = new List<GameObject>();


        public void Awake()
        {
            // Find all tanks from the GreenPlatoon
            tanks = new List<GameObject>(GameObject.FindGameObjectsWithTag("GreenTank"));
        }

        public GameObject getEnemyTarget()
        {
            return targets.Count > 0 ? targets[0] : null;
        }

        public List<GameObject> getCurrentTargets()
        {
            return targets;
        }

        public void enemySpotted(GameObject enemy)
        {
            spottedEnemies.Add(enemy);
        }


        public List<GameObject> getAliveTanks()
        {
            return aliveTanks;
        }


        public Vector3 getPlatoonMeanPosition()
        {
            return platoonMeanPosition;
        }

        private void calcTarget()
        {
            targets = new List<GameObject>(spottedEnemies);
            targets.Sort(new TargetComparer(platoonMeanPosition));
            spottedEnemies = new HashSet<GameObject>();
        }

        private void calcAliveTanks()
        {
            aliveTanks = new List<GameObject>();
            foreach (var tank in tanks)
                if (tank.GetComponent<TankController>().health > 0)
                    aliveTanks.Add(tank);
        }

        private void calcPlatoonMeanPosition()
        {
            if (aliveTanks.Count > 0)
            {
                var mean = new Vector3();
                foreach (var tank in aliveTanks)
                    mean += tank.transform.position;

                platoonMeanPosition = mean / aliveTanks.Count;
            }
            else
                platoonMeanPosition = Vector3.zero;
        }

        protected void LateUpdate()
        {
            // calc platoon info
            calcAliveTanks();
            calcPlatoonMeanPosition();

            // calc enemy info
            calcTarget();
            
            Debug.Log(this.spottedEnemies);
        }
    }

    /// <summary>
    /// TargetComparer
    ///
    /// used to sort all targets by  priority
    /// 
    /// <<param name="platoonPosition">
    /// The position of the platoon used to calculate the distance to the targets.
    /// </param>
    /// </summary>
    public class TargetComparer : Comparer<GameObject>
    {
        private readonly Vector3 platoonPosition;

        public TargetComparer(Vector3 platoonPosition)
        {
            this.platoonPosition = platoonPosition;
        }

        public override int Compare(GameObject target1, GameObject target2)
        {
            // use sqrMagnitude instead of Vector3.distance which is the 
            // same as (a - b).magnitude which is more expensive
            return (int) ((platoonPosition - target1.transform.position).sqrMagnitude -
                          (platoonPosition - target2.transform.position).sqrMagnitude);
        }
    }
}