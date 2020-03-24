using System.Collections.Generic;
using UnityEngine;

namespace Green
{
    public interface IPlatoonController
    {
        GameObject getEnemyTarget();
        List<GameObject> getCurrentTargets();
        void enemySpotted(GameObject enemy);
        List<GameObject> getAliveTanks();
        Vector3 getPlatoonMeanPosition();
    }
}