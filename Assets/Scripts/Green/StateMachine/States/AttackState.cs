using System;
using GreenStateMachine;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Green
{
    public class AttackState : State<TankState>
    {
        public GameObject Bullet;

        private TankController tankController;
        private GameObject[] targets;
        private GameObject currentTarget;
        protected float elapsedTime;
        private float shootRate;
        private GameObject tank;

        private Transform bulletSpawnPoint;
        private Transform turret;
        private IPlatoonController platoonController;

        public AttackState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            this.tankController = tankController;
            this.tank = gameObject;
            this.platoonController = tankController.platoonController;
        }

        public override TankState? act()
        {
            tankController.Shoot();
            
            // no more enemies around -> regroup
            if (platoonController.getCurrentTargets().Count <= 0)
            {
                return TankState.REGROUPING;
            }

            // enemies are are around but not in attack range -> chase
            if (platoonController.getCurrentTargets().Count > 0 &&
                tankController.EnemiesInAttackRange().Count <= 0)
            {
                return TankState.CHASE;
            }

            // aim and if target is locked -> shoot
            if (tankController.Aim(platoonController.getEnemyTarget()))
            {
                Debug.Log("Target in sights");
                // if enough time to execute a dodge maneuver -> do it
                if (tankController.Shoot() > 2.0f)
                {
                    dodgeAttacks();
                }
            }

            return null;
        }

        //Moves the tank around with the goal of dodging incoming bullets
        public void dodgeAttacks()
        {
            Debug.Log("Moving forwards");
            gameObject.transform.position += gameObject.transform.forward * Time.deltaTime * tankController.maxForwardSpeed;
        }
    }
}