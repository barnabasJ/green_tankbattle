using System;
using GreenStateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

namespace Green
{
    public class AttackState : State<TankState>
    {
        public GameObject Bullet;

        private TankController tankController;
        private GameObject[] targets;
        private GameObject currentTarget;
        private float elapsedTime;
        private float totalTime;
        private float shootRate;
        private GameObject tank;
        private Boolean moving = false;

        private Transform bulletSpawnPoint;
        private Transform turret;
        private IPlatoonController platoonController;
        private int enemiesInRange;
        private int currentTargets;
        private Vector3 destination;

        public AttackState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            this.tankController = tankController;
            this.tank = gameObject;
            this.platoonController = tankController.platoonController;
        }

        public override TankState? act()
        {
            //Check for current targets and enemies in range.
            currentTargets = platoonController.getCurrentTargets().Count;
            enemiesInRange = tankController.EnemiesInAttackRange().Count;
            
            // aim and if target is locked -> shoot
            currentTarget = platoonController.getEnemyTarget();
            tankController.Aim();

            //Check line of sight
            if (tankController.checkLineOfSight())
            {
                //Line of sight clear, dodge and shoot
                tankController.Shoot();
            }
            else
            {
                return TankState.EVADING;
            }

            if ( enemiesInRange == 0 && currentTargets != 0)
            {
                return TankState.CHASE;
            }
                
            // no more enemies around -> regroup
            if ( enemiesInRange == 0 && currentTargets == 0)
            {
                return TankState.REGROUPING;
            }
            
            //Move towards the calculated destination
            tankController.GetComponent<NavMeshAgent>().destination = destination * (tankController.GetComponent<NavMeshAgent>().stoppingDistance + 10);

            return null;
        }
    }
}