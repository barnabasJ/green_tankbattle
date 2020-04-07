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
        private Vector3 aimStartPos;

        private StateMachine<TankState> stateMachine;
        public PlatoonController platoonController { get; private set; }

        // config
        //public int health { get; private set; }
        public int health;
        public float curSpeed { get; private set; }
        public float targetSpeed { get; private set; }
        private Vector3 targetPreviousPos;
        public float rotSpeed { get; private set; } = 10f;
        public float turretRotSpeed { get; } = 10.0f;
        public float maxForwardSpeed { get; } = 300.0f;
        public float maxBackwardSpeed { get; } = -300.0f;
        public float minAttackRange { get; } = 50f;
        public float maxAttackRange { get; } = 300f;
        public float spottingDistace { get; } = 500f;
        public float shootRate { get; } = 3.0f;

        // stuff needed for logic 
        private float elapsedTime;
        public float evadeDistance { get; } = 10f;

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
                GameObject.FindWithTag("GreenPlatoonController").GetComponent<PlatoonController>();

            var stateMap = new Dictionary<TankState, State<TankState>>
            {
                {TankState.PATROLLING, new PatrolState(gameObject, this)},
                {TankState.CHASE, new ChaseState(gameObject, this)},
                {
                    TankState.ATTACKING,
                    new AttackState(gameObject, this)
                },
                {
                    TankState.DEAD,
                    new DeathState(gameObject, this)
                },
                {TankState.EVADING, new EvadeState(gameObject, this)},
                {TankState.REGROUPING, new RegroupState(gameObject, this)},
                {TankState.FLEE, new FleeState(gameObject, this)}
            };

            stateMachine = new StateMachine<TankState>(stateMap);
            stateMachine.transition(TankState.ATTACKING);
        }

        protected void Update()
        {
            if (health <= 0)
            {
                stateMachine.transition(TankState.DEAD);
            }
            
            if (tanksInCrashDistance(evadeDistance).Count > 0)
                stateMachine.transition(TankState.EVADING);

            
            foreach (var enemyCollider in SpottedEnemies())
            {
                platoonController.enemySpotted(enemyCollider.gameObject);
            }

            if (SpottedEnemies().Count >= platoonController.getAliveTanks().Count)
            {
                stateMachine.transition(TankState.FLEE);
            }


            stateMachine.transition(stateMachine.act());
            
        }

        void OnCollisionEnter(Collision collision)
        {
            //Reduce health
            if (collision.gameObject.CompareTag("Bullet"))
            {
                health -= 5;
                Debug.Log(health);
            }
        }

        public void Start()
        {
            GetComponent<NavMeshAgent>().isStopped = false;
        }

        public void Stop()
        {
            GetComponent<NavMeshAgent>().isStopped = true;
        }

        public void Aim()
        {
            GameObject target = platoonController.getEnemyTarget();

            if (target == null)
            {
                return;
            }
            
            //Get the aim starting position
            aimStartPos = new Vector3(turret.transform.position.x + 5f, turret.transform.position.y - 0.5f, turret.transform.position.z);
            
            //Calculate the target velocity
            var velocity = (target.transform.position - targetPreviousPos) / Time.deltaTime;
            
            Vector3 targetPos = new Vector3(target.transform.position.x,target.transform.position.y +1.5f, target.transform.position.z);
            
            // Determine which direction to rotate towards
            Vector3 targetDirection = FindInterceptVector(aimStartPos, 600f, targetPos, velocity);
            // The step size is equal to speed times frame time.
            float singleStep = turretRotSpeed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(turret.transform.forward, targetDirection, singleStep, 0.0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            turret.transform.rotation = Quaternion.LookRotation(newDirection);

            targetPreviousPos = target.transform.position;
        }
        
        private Vector3 FindInterceptVector(Vector3 shotOrigin, float shotSpeed,
            Vector3 targetOrigin, Vector3 targetVel) {
   
            Vector3 dirToTarget = Vector3.Normalize(targetOrigin - shotOrigin);
   
            // Decompose the target's velocity into the part parallel to the
            // direction to the cannon and the part tangential to it.
            // The part towards the cannon is found by projecting the target's
            // velocity on dirToTarget using a dot product.
            Vector3 targetVelOrth =
                Vector3.Dot(targetVel, dirToTarget) * dirToTarget;
   
            // The tangential part is then found by subtracting the
            // result from the target velocity.
            Vector3 targetVelTang = targetVel - targetVelOrth;

            // The tangential component of the velocities should be the same
            // (or there is no chance to hit)
            // THIS IS THE MAIN INSIGHT!
            Vector3 shotVelTang = targetVelTang;
   
            // Now all we have to find is the orthogonal velocity of the shot
   
            float shotVelSpeed = shotVelTang.magnitude;
            if (shotVelSpeed > shotSpeed) {
                // Shot is too slow to intercept target, it will never catch up.
                // Do our best by aiming in the direction of the targets velocity.
                return targetVel.normalized * shotSpeed;
            } else {
                // We know the shot speed, and the tangential velocity.
                // Using pythagoras we can find the orthogonal velocity.
                float shotSpeedOrth =
                    Mathf.Sqrt(shotSpeed * shotSpeed - shotVelSpeed * shotVelSpeed);
                Vector3 shotVelOrth = dirToTarget * shotSpeedOrth;
       
                // Finally, add the tangential and orthogonal velocities.
                return shotVelOrth + shotVelTang;
            }
        }

        public Boolean checkLineOfSight()
        {
            RaycastHit hit;
            Debug.DrawRay(aimStartPos, turret.forward * maxAttackRange, Color.blue);
            if (Physics.Raycast(aimStartPos, turret.forward * maxAttackRange, out hit))
            {
                if (!hit.collider.CompareTag("GreenTank"))
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Shoots a bullet if enough time is elapsed
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

        public List<Collider> tanksInCrashDistance(float distance)
        {
            return checkSurroundings(distance, isOtherTank);
        }

        private bool isOtherTank(Collider collider)
        {
            return !collider.gameObject.Equals(gameObject) && collider.gameObject.GetComponent<NavMeshAgent>() != null;
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

        // return the mean  position of all enemy tanks in the vicinity, used for chasing or fleeing
        public Vector3 EnemyMeanPosition() {
            List<Collider> spottedEnemies = SpottedEnemies();
            Vector3 sumPosition = new Vector3(0,0,0);
            foreach(Collider enemy in spottedEnemies) {
                sumPosition += enemy.gameObject.transform.position;
            }
            return sumPosition/spottedEnemies.Count;
        }
    }
}