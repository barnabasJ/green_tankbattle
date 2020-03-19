using GreenStateMachine;
using UnityEngine;

namespace Green
{
    public class PatrolState: State<TankState>
    {
        private TankController tankController;

        public PatrolState(GameObject gameObject, TankController tankcontroller) : base(gameObject)
        {
            this.tankController = tankController;
        }

        public override TankState? act()
        {
            throw new System.NotImplementedException();
        }
    }
}