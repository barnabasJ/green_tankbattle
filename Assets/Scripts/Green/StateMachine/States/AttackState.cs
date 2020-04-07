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
            if (elapsedTime >= 10)
            {
                //Reset the time
                elapsedTime = 0.0f;
            }
            else
            {
                elapsedTime += Time.deltaTime;
            }

            // aim and if target is locked -> shoot
            currentTarget = platoonController.getEnemyTarget();
            tankController.Aim();

            //Check line of sight
            if (tankController.checkLineOfSight())
            {
                Debug.Log("Line of sight clear");
                //Line of sight clear, dodge and shoot
                tankController.Shoot();
            }
            else
            {
                return TankState.EVADING;
            }

            if ( enemiesInRange == 0 && currentTargets != 0 && elapsedTime > 10)
            {
                return TankState.CHASE;
            }
                
            // no more enemies around -> regroup
            if ( enemiesInRange == 0 && currentTargets == 0 && elapsedTime > 10)
            {
                return TankState.REGROUPING;
            }
            
            //Move towards the calculated destination
            tankController.GetComponent<NavMeshAgent>().destination = destination * (tankController.GetComponent<NavMeshAgent>().stoppingDistance + 10);

            return null;
        }

        public Vector3 moveOutOfTheWay()
        {
            float random = Random.Range(-10.0f, 10.0f);
            float x = gameObject.transform.position.x + random;
            float y = gameObject.transform.position.y + random;
            return  new Vector3(x, y, gameObject.transform.position.z);
        }

        //Moves the tank around with the goal of dodging incoming bullets
        public Vector3 dodgeAttacks()
        {
            totalTime += Time.deltaTime * tankController.maxForwardSpeed;
            float x = Mathf.Cos(totalTime) * 5;
            x += currentTarget.transform.position.x;
            return new Vector3(x + currentTarget.transform.position.x, currentTarget.transform.position.y , currentTarget.transform.position.z);
        }
    }
}