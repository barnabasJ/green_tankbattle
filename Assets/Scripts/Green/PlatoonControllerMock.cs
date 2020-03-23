using UnityEngine;

namespace Green
{
    //delete this class on integration
    public class PlatoonControllerMock : MonoBehaviour, IPlatoonController
    {
        public GameObject getEnemyTarget()
        {
            throw new System.NotImplementedException();
        }

        public int getCurrentEnemyCount()
        {
            return 0;
        }

        public void enemySpotted(GameObject enemy)
        {
            throw new System.NotImplementedException();
        }

        public int getPlatoonCount()
        {
            throw new System.NotImplementedException();
        }

        public int getAliveTanksCount()
        {
            throw new System.NotImplementedException();
        }

        public void reportForDuty(GameObject tank)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 getPlatoonMeanPosition()
        {
            throw new System.NotImplementedException();
        }
    }
}