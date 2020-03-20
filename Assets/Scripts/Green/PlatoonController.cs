using System.Collections.Generic;
using UnityEngine;

namespace Green
{
    public class PlatoonController : MonoBehaviour, IPlatoonController
    {
        private List<GameObject> aliveTanks = new List<GameObject>();
        private Vector3 platoonMeanPosition;
        private readonly List<GameObject> spottedEnemies = new List<GameObject>();
        private readonly List<GameObject> tanks = new List<GameObject>();
        private GameObject target;

        public GameObject getEnemyTarget()
        {
            return target;
        }

        public int getCurrentEnemyCount()
        {
            return spottedEnemies.Count;
        }

        public void enemySpotted(GameObject enemy)
        {
            spottedEnemies.Add(enemy);
        }

        public int getPlatoonCount()
        {
            return tanks.Count;
        }

        public int getAliveTanksCount()
        {
            return aliveTanks.Count;
        }

        public void reportForDuty(GameObject tank)
        {
            tanks.Add(tank);
        }

        public Vector3 getPlatoonMeanPosition()
        {
            return platoonMeanPosition;
        }

        private void calcTarget()
        {
            if (spottedEnemies.Count <= 0)
                this.target = null;

            var target = spottedEnemies[0];
            var shortestDistance = Vector3.Distance(platoonMeanPosition, spottedEnemies[0].transform.position);
            for (var i = 1; i < spottedEnemies.Count; i++)
            {
                var currentDistance = Vector3.Distance(platoonMeanPosition, spottedEnemies[i].transform.position);
                if (currentDistance < shortestDistance)
                {
                    shortestDistance = currentDistance;
                    target = spottedEnemies[i];
                }
            }

            this.target = target;
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
            var mean = new Vector3();
            foreach (var tank in tanks)
                mean += tank.transform.position;
            if (tanks.Count > 0)
                platoonMeanPosition = mean /= tanks.Count;
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
        }
    }
}