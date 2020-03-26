using System;
using System.Collections.Generic;
using System.Linq;
using GreenStateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Green
{
    public enum TankState
    {
        PATROLLING,
        CHASE,
        EVADING,
        FLEE,
        DEAD,
        ATTACKING,
        REGROUPING,
    }

    public class TankController : MonoBehaviour
    {
        // objects
        public GameObject Bullet;
        private Transform turret;
        private Transform bulletSpawnPoint;

        private StateMachine<TankState> stateMachine;
        public IPlatoonController platoonController { get; private set; }

        // config
        public int health { get; private set; }
        public float curSpeed { get; private set; }
        public float targetSpeed { get; private set; }
        public float rotSpeed { get; private set; }
        public float turretRotSpeed { get; private set; } = 10.0f;
        public float maxForwardSpeed { get; private set; } = 300.0f;
        public float maxBackwardSpeed { get; private set; } = -300.0f;
        public float minAttackRange { get; private set; } = 50f;
        public float maxAttackRange { get; private set; } = 300f;
        public float spottingDistace { get; private set; } = 500f;
        public float shootRate { get; private set; } = 3.0f;

        // stuff needed for logic 
        private float elapsedTime;

        //Initialize the Finite state machine for the NPC tank
        protected void Awake()
        {
            health = 100;
            elapsedTime = 0.0f;
            rotSpeed = 150.0f;

            //Get the target enemy(Player)

            //Get the turret of the tank
            turret = gameObject.transform.GetChild(0).transform;
            bulletSpawnPoint = turret.GetChild(0).transform;

            platoonController =
                GameObject.FindWithTag("GreenPlatoonController").GetComponent<IPlatoonController>();

            var stateMap = new Dictionary<TankState, State<TankState>>
            {
                {TankState.PATROLLING, new PatrolState(gameObject, this)},
                {TankState.CHASE, new ChaseState(gameObject, this)},
                {
                    TankState.ATTACKING,
                    new AttackState(gameObject, this)
                },
                {TankState.REGROUPING, new RegroupState(gameObject, this)}
            };

            stateMachine = new StateMachine<TankState>(stateMap);
            stateMachine.transition(TankState.ATTACKING);
        }

        protected void Update()
        {
            foreach (var enemyCollider in SpottedEnemies())
            {
               platoonController.enemySpotted(enemyCollider.gameObject); 
            }

            stateMachine.transition(stateMachine.act());
        }


        // when the platoon spot an enemy, compare the enemy power to the platoon power
        // if enemy power higher, switch to flee
        // if enemy power lower, switch to chase
        // TODO: Write check enemy power function
        bool EnemyPowerIsLow()
        {
            return false;
        }

        // return an enemy who is up for kill
        public GameObject CurrentChaseEnemy()
        {
            return null;
        }


        void OnCollisionEnter(Collision collision)
        {
            //Reduce health
            if (collision.gameObject.CompareTag("Bullet"))
            {
                health -= 5;
                Debug.Log("Tank health: " + health);
                if (health <= 50)
                {
                    stateMachine.transition(TankState.FLEE);
                }
                else if (health <= 0)
                {
                    stateMachine.transition(TankState.DEAD);
                }
            }
        }

        public bool Aim(GameObject target, float interceptorSpeed)
        {
            var aimPostion = calAimPosition(target, interceptorSpeed);
            // can't aim at target 
            if (aimPostion == null)
                return false;

            // aiming correctly
            if (bulletSpawnPoint.transform.position == aimPostion)
                return true;

            // TODO: rotate the turret
            Quaternion lookRotation = Quaternion.LookRotation((Vector3) aimPostion);
            turret.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turretRotSpeed);
            return false;
        }

        private Vector3? calAimPosition(GameObject target, float aInterceptorSpeed)
        {
            Vector3 targetPosition = target.transform.position;
            Vector3 targetSpeed = target.GetComponent<Rigidbody>().velocity;
            
            //Set target direction
            Vector3 targetDir = targetPosition - transform.position;

            float iSpeed2 = aInterceptorSpeed * aInterceptorSpeed;

            //Calculating with square magnitute is faster.
            float tSpeed2 = targetSpeed.sqrMagnitude;


            float fDot1 = Vector3.Dot(targetDir, targetSpeed);
            float targetDist2 = targetDir.sqrMagnitude;
            float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
            if (d < 0.1f) // negative means that no possible course is valid because the interceptor isn't fast enough
                return null;
            float sqrt = Mathf.Sqrt(d);
            float S1 = (-fDot1 - sqrt) / targetDist2;
            float S2 = (-fDot1 + sqrt) / targetDist2;
            if (S1 < 0.0001f)
            {
                if (S2 < 0.0001f)
                    return null;
                else
                    return (S2) * targetDir + targetSpeed;
            }
            else if (S2 < 0.0001f)
                return (S1) * targetDir + targetSpeed;
            else if (S1 < S2)
                return (S2) * targetDir + targetSpeed;
            else
                return (S1) * targetDir + targetSpeed;
        }

        /// <summary>
        /// Shots a bullet if enough time is elapsed
        /// </summary>
        /// <returns>The time until shooting again is possible</returns>
        public float Shoot()
        {
            if (elapsedTime >= shootRate)
            {
                //Reset the time
                elapsedTime = 0.0f;


                //Also Instantiate over the PhotonNetwork
                Instantiate(Bullet, bulletSpawnPoint.position + bulletSpawnPoint.forward, bulletSpawnPoint.rotation);
            }
            else
            {
                elapsedTime += Time.deltaTime;
            }

            return shootRate - elapsedTime;
        }

        public List<Collider> EnemiesInAttackRange()
        {
            // enemies that are between max and min range;
            return checkSurroundings(maxAttackRange, isEnemyTank).Except(
                checkSurroundings(minAttackRange - 1, isEnemyTank)).ToList();
        }

        public List<Collider> SpottedEnemies()
        {
            return checkSurroundings(spottingDistace, isEnemyTank);
        }

        private bool isEnemyTank(Collider collider)
        {
            // it is not a friendly tank and it has a NavMeshAgent -> it's an enemy
            return !collider.gameObject.CompareTag("GreenTank") &&
                   collider.gameObject.GetComponent<NavMeshAgent>() != null;
        }


        private List<Collider> checkSurroundings(float radius, Func<Collider, bool> filter)
        {
            List<Collider> objects = new List<Collider>();
            var colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (var collider in colliders)
            {
                if (filter(collider))
                {
                    objects.Add(collider);
                }
            }

            return objects;
        }
    }
}