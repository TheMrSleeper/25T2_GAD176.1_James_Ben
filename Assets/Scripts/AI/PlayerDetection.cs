using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ShooterFramework.AI
{
    public class PlayerDetection : MonoBehaviour
    {
        static public bool found = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                found = true;
                print("Player has enterred dettection radius.");
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                found = false;
                print("Player has left detection radius.");
            }
        }
    }
}
