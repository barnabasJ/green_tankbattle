using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Green
{
    public class PlatoonController : MonoBehaviour, IPlatoonController
    {
        private List<GameObject> tanks = new List<GameObject>();
        private List<GameObject> aliveTanks = new List<GameObject>();
        private List<GameObject> spottedEnemies = new List<GameObject>();
        private GameObject target = null;
        private Vector3 platoonMeanPosition;

        public GameObject getEnemyTarget()
        {
            return this.target;
        }

        private void calcTarget()
        {
            if (spottedEnemies.Count <= 0)
                this.target = null;

            var target = spottedEnemies[0];
            var shortestDistance = Vector3.Distance(this.platoonMeanPosition, spottedEnemies[0].transform.position);
            for (var i = 1; i < spottedEnemies.Count; i++)
            {
                var currentDistance = Vector3.Distance(this.platoonMeanPosition, spottedEnemies[i].transform.position);
                if (currentDistance < shortestDistance)
                {
                    shortestDistance = currentDistance;
                    target = spottedEnemies[i];
                }
            }

            this.target = target;
        }

        public int getCurrentEnemyCount()
        {
            return this.spottedEnemies.Count;
        }

        public void enemySpotted(GameObject enemy)
        {
            this.spottedEnemies.Add(enemy);
        }

        public int getPlatoonCount()
        {
            return this.tanks.Count;
        }

        public int getAliveTanksCount()
        {
            return this.aliveTanks.Count;
        }

        private void calcAliveTanks()
        {
            this.aliveTanks = new List<GameObject>();
            foreach (var tank in tanks)
                if (tank.GetComponent<TankController>().health > 0)
                    aliveTanks.Add(tank);
        }

        public void reportForDuty(GameObject tank)
        {
            this.tanks.Add(tank);
        }

        public Vector3 getPlatoonMeanPosition()
        {
            return this.platoonMeanPosition;
        }

        private void calcPlatoonMeanPosition()
        {
            Vector3 mean = new Vector3();
            foreach (var tank in tanks)
                mean += tank.transform.position;
            if (tanks.Count > 0)
                this.platoonMeanPosition = mean /= tanks.Count;
            else
                this.platoonMeanPosition = Vector3.zero;
        }

        protected void LateUpdate()
        {
            // calc platoon info
            this.calcAliveTanks();
            this.calcPlatoonMeanPosition();

            // calc enemy info
            this.calcTarget();
        }
    }
}