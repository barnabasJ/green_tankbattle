using UnityEngine;

namespace Green
{
    public interface IPlatoonController
    {
        GameObject getEnemyTarget();
        int getCurrentEnemyCount();
        void enemySpotted(GameObject enemy);
        int getPlatoonCount();
        int getAliveTanksCount();
        void reportForDuty(GameObject tank);
        Vector3 getPlatoonMeanPosition();
    }
}