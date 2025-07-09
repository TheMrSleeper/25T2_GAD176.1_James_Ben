using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShooterFramework.AI
{
    public class EnemyBehaviour : MonoBehaviour
    {
        bool lookAt = false;
        [SerializeField] GameObject player;

        void Update()
        {
            if (PlayerDetection.found)
            {
                lookAt = true;
            }
            if (lookAt)
            {
                transform.LookAt(player.transform);
            }
        }
    }
}
