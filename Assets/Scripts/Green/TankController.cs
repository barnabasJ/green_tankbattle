using System.Collections.Generic;
using GreenStateMachine;
using UnityEngine;

namespace Green
{
    public enum TankState
    {
        PATROLING,
        CHASE,
        EVADING
    }

    public class TankController : MonoBehaviour
    {
        public GameObject Bullet;
        public int health { get; private set; }
        private float elapsedTime;
        private float shootRate;
        private Transform turret;
        private Transform bulletSpawnPoint;
        private StateMachine<TankState> stateMachine;
        private IPlatoonController platoonController;

        //Initialize the Finite state machine for the NPC tank
        protected void Awake()
        {
            health = 100;
            elapsedTime = 0.0f;
            shootRate = 2.0f;

            //Get the target enemy(Player)

            //Get the turret of the tank
            turret = gameObject.transform.GetChild(0).transform;
            bulletSpawnPoint = turret.GetChild(0).transform;

            platoonController =
                GameObject.FindWithTag("PlatoonController").GetComponent<IPlatoonController>();

            var stateMap = new Dictionary<TankState, State<TankState>>
            {
                {TankState.PATROLING, new PatrolState(gameObject, this)},
                {TankState.CHASE,  new ChaseState(gameObject, this)}
            };

            stateMachine = new StateMachine<TankState>(stateMap);
        }

        //Update each frame
        protected void Update()
        {
            if(EnemyPowerIsLow()) {
                stateMachine.transition(TankState.CHASE);
            }
            stateMachine.transition(stateMachine.act());
        }
        

        // when the platoon spot an enemy, compare the enemy power to the platoon power
        // if enemy power higher, switch to flee
        // if enemy power lower, switch to chase
        // TODO: Write check enemy power function
        bool EnemyPowerIsLow() {
            return false;
        }

        // return an enemy who is up for kill
        public GameObject CurrentChaseEnemy(){
            return null;
        }
        
    }
}