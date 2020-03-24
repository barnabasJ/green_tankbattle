using System;
using GreenStateMachine;
using UnityEngine;
using System.Collections;

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
        
        private Transform bulletSpawnPoint;
        private Transform turret;
        
        public AttackState(GameObject gameObject, TankController givenTankController, GameObject[] targetList, float shootRate, Transform bulletSpawnPoint, Transform turret) : base(gameObject)
        {
            tankController = givenTankController;
            targets = targetList;
            elapsedTime = elapsedTime;
            turret = turret;
        }

        public override TankState? act()
        {
            //While any of the enemies are within target range, target and shoot them.
            
            // Should go into regroup or flee.

            //If no enemies are within shooting range, but one is still in spotting range, chase it.
            
            throw new System.NotImplementedException();
        }

        private void selectTarget()
        {
            //Always target first index in target array.
            currentTarget = targets[0];

            Vector3 targetPosition = currentTarget.transform.position;
            Vector3 targetSpeed = currentTarget.GetComponent<Rigidbody>().velocity;
            
            //Take aim at the targets position. Start from current tank position. 
            Vector3 interceptPosition = aim(targetPosition, targetSpeed, tankController.transform.position, 10f);
        }
        
        //Check if the target still exist?
        private Boolean targetExists()
        {
            return false;
        }

        //Moves the tank around with the goal of dodging incoming bullets
        public void dodgeAttacks()
        {
            
        }

        private Vector3 aim(Vector3 aTargetPos, Vector3 aTargetSpeed, Vector3 aInterceptorPos, float aInterceptorSpeed)
        {
            //Set target direction
            Vector3 targetDir = aTargetPos - aInterceptorPos;
            
            float iSpeed2 = aInterceptorSpeed * aInterceptorSpeed;
            
            //Calculating with square magnitute is faster.
            float tSpeed2 = aTargetSpeed.sqrMagnitude;
            
            
            float fDot1 = Vector3.Dot(targetDir, aTargetSpeed);
            float targetDist2 = targetDir.sqrMagnitude;
            float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
            if (d < 0.1f)  // negative means that no possible course is valid because the interceptor isn't fast enough
                return Vector3.zero;
            float sqrt = Mathf.Sqrt(d);
            float S1 = (-fDot1 - sqrt) / targetDist2;
            float S2 = (-fDot1 + sqrt) / targetDist2;
            if (S1 < 0.0001f)
            {
                if (S2 < 0.0001f)
                    return Vector3.zero;
                else
                    return (S2) * targetDir + aTargetSpeed;
            }
            else if (S2 < 0.0001f)
                return (S1) * targetDir + aTargetSpeed;
            else if (S1 < S2)
                return (S2) * targetDir + aTargetSpeed;
            else
                return (S1) * targetDir + aTargetSpeed;
        }
        
        /*!TODO I cannot instantiate a bullet in this class.*/
        void UpdateWeapon()
        {
            if(Input.GetMouseButton(0))
            {
                if (elapsedTime >= shootRate)
                {
                    //Reset the time
                    elapsedTime = 0.0f;

                    //Also Instantiate over the PhotonNetwork
                    tankController.Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                }
            }
        }
    }
}