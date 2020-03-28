using System.Collections;
using System.Collections.Generic;
using GreenStateMachine;
using UnityEngine;
using Green;
using UnityEngine.AI;

public class DeathState : State<TankState>
{
    private TankController tankController;

        public DeathState(GameObject gameObject, TankController tankController) : base(gameObject)
        {
            this.tankController = tankController;
        }
        
        // when tank enter death state, stop it, then set it to passive
        public override void onStateEnter()
        {
            tankController.Stop();
            this.tankController.gameObject.SetActive(false);
        }

        public override TankState? act()
        {
            return null;
        }
}
