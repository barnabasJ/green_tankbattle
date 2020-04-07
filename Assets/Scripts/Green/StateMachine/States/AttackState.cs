using System;
using GreenStateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

namespace Green
{
    public class AttackState : State<TankState>
    {
        public GameObject Bullet;

        private TankController tankController;
        private GameObject[] targets;
        private GameObject currentTarget;
        private float elapsedTime;
        private float shootRate;
        private GameObject tank;
        private Boolean moving = false;

        private Transform bulletSpawnPoint;
        private Transform turret;
        private IPlatoonController platoonController;
        private int enemiesInRange;
        private int currentTargets;

        public AttackState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            this.tankController = tankController;
            this.tank = gameObject;
            this.platoonController = tankController.platoonController;
        }

        public override TankState? act()
        {
            if (elapsedTime >= 10)
            {
                //Reset the time
                elapsedTime = 0.0f;
            }
            else
            {
                elapsedTime += Time.deltaTime;
            }

            // enemies are are around but not in attack range -> chase
            currentTargets = platoonController.getCurrentTargets().Count;
            enemiesInRange = tankController.EnemiesInAttackRange().Count;
            
            if ( enemiesInRange == 0 && currentTargets != 0)
            {
                return TankState.CHASE;
            }
                
            // no more enemies around -> regroup
            if ( enemiesInRange == 0 && currentTargets == 0)
            {
                return TankState.REGROUPING;
            }
            
            // aim and if target is locked -> shoot
            currentTarget = platoonController.getEnemyTarget();
            tankController.Aim(currentTarget);
            if (tankController.checkLineOfSight())
            {
                moving = false;
            }
            else
            {
                moving = true;
            }

            if (!moving)
            {
                tankController.Shoot();
            }


            if (moving)
            {
                dodgeAttacks();
            }

            return null;
        }

        //Moves the tank around with the goal of dodging incoming bullets
        public void dodgeAttacks()
        {
            //Debug.Log("Moving");
            moving = true;
            Vector3 dest = new Vector3(gameObject.transform.position.x + 100f, gameObject.transform.transform.position.y, gameObject.transform.transform.position.z);
            tankController.GetComponent<NavMeshAgent>().destination = dest;
        }
    }
}