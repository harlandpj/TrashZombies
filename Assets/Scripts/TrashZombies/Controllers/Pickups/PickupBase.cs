using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TrashZombies.Pickups
{
    /// <summary>
    /// class which is the base class for all pickups
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PickupBase : MonoBehaviour
    {
        [SerializeField]
        protected PickupStats pickupStats; // available to subclasses

        [SerializeField]
        AudioClip pointsClip;
        
        protected AudioSource audioSource;

        private bool hitByPlayer = false;
        private bool bThrownByNPC = false; // this pickup object can also be thrown by an NPC

        // accessor
        public PickupStats GetPickupStats()
        {
            return pickupStats;
        }

        public void SetThrownByNPC(bool npcThrow)
        {
            bThrownByNPC = npcThrow;
        }

        protected void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        // player
        GameObject Player;
        bool hitTriggerOnce = false;

        private void Update()
        {
            if (transform.position.y < -2f)
            {
                // fallen thru floor, return pickup to pickup pool
                bThrownByNPC = false;
                gameObject.SetActive(false);    
                PickupPoolManager.Instance.ReturnPickupToPool(gameObject);
            }                
        }

        // Stop object falling through terrain mesh
        // Haven't found any other solution - mesh thickness / continuous collider checking etc
        // as need it to have a rigid body and use gravity, but Unity mesh detection not good enough
        protected void OnTriggerEnter(Collider other)
        {
            // not ideal, as any rotating (x,y,z) direction body will simply land at the angle it was at
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

            gameObject.transform.position = new Vector3(
                gameObject.transform.position.x,
                gameObject.GetComponent<Renderer>().bounds.center.y+0.1f, 
                gameObject.transform.position.z);

            if (!GameController.Instance.m_bGameOver)
            {
                // game running, check who triggered and if player update score        
                if (other.gameObject.CompareTag("Player") && !hitByPlayer)
                {
                    // turn collider off for extra security
                    gameObject.GetComponent<CapsuleCollider>().enabled = false;

                    StartPickupCollected();
                }
                else if (Player.transform.position.y - transform.position.y < 0.15f)
                {
                    gameObject.GetComponent<Rigidbody>().useGravity = false;

                    if (!hitTriggerOnce && !bThrownByNPC)
                    {
                        hitTriggerOnce = true;
                        GameController.CityHealth -= 0.1f; // reduce city health!
                    }
                }
            }

            if (transform.position.y < -1f)
            {
                // return to pool as fell thru floor or maybe stuck in air after being thrown by an NPC
                bThrownByNPC = false;
                gameObject.SetActive(false);    
                PickupPoolManager.Instance.ReturnPickupToPool(gameObject);
            }
        }
        
        protected void StartPickupCollected()
        {
            // prevent re-collisions giving more points
            hitByPlayer = true;

            if (bThrownByNPC)
            {
                gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
            
            // turn collider off for extra security
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            

            // play collect noise
            audioSource.clip = pointsClip;
            audioSource.PlayOneShot(pointsClip, 0.8f); // play collected sound

            StartCoroutine(PickupCollected());
        }

        // update score and return to pool manager
        IEnumerator PickupCollected()
        {
            if (!bThrownByNPC)
            {
                // only score if we picked it up
                GameController.Score += pickupStats.PickupValue;
                GameController.CityHealth += 0.1f;            }
            else
            {
                // reduce health by 1%
                GameController.Health -=1;
                bThrownByNPC= false;
            }

            yield return new WaitForSeconds(0.5f);
            Debug.Log("Pickup Collected!");

            // return pickup to pickup pool
            gameObject.SetActive(false); 
            PickupPoolManager.Instance.ReturnPickupToPool(gameObject);
        }
    }
}