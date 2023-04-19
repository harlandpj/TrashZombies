using System.Collections;
using System.Collections.Generic;
using TrashZombies;
using UnityEngine;

using TrashZombies.Pickups;
using System.Runtime.CompilerServices;

/// <summary>
/// Pool manager that maintains a list of "rubbish" game objects which can be thrown
/// or dropped by the Player, or spawned at intervals
/// This is a SINGLETON class.
/// </summary>
public class PickupPoolManager : MonoBehaviour
{
    //[Header]
    public GameObject[] pickups = new GameObject[5]; // drag gameobjects in using editor to be instantiated later

    private List<GameObject> pickupPool = new List<GameObject>();

    [SerializeField]
    int poolSize = 1000;  // default to 1000 objects - but can be changed in editor

    private bool pickupPoolInitialised = false;

    /// <summary>
    /// Instance function for access to instance
    /// </summary>
    public static PickupPoolManager Instance { get; private set; }

    /// <summary>
    /// setup static Instance object (if not there) and retain data on (possible other) scene loading
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
        }

        // ensure not destroyed on (**future dev***) any subsequent scene load
        //DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        // setup list of objects to spawn
        if (!pickupPoolInitialised)
        {
            pickupPoolInitialised = true;
            SetupPickupPool();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // don't spawn if game not running    
        if (!GameController.Instance.m_bGameStarted ||
            GameController.Instance.m_bGamePaused)
        {
            // don't spawn
        }
        else
        {
            // ok to spawn periodically
        }
    }

    // use for locking list access, spawner may not spawn but safer than an
    // inconsistent list state
    static object ListLocker = new object();

    /// <summary>
    /// Setup Pool of trash objects
    /// </summary>
    void SetupPickupPool()
    {
        int pickupChoices = pickups.Length;
        GameObject randomObj;
        GameObject newObj;
        
        for (int poolCount=0; poolCount < poolSize; poolCount++)
        {
            randomObj = pickups[Random.Range(0, pickupChoices - 1)];

            // add trash object to pool
            newObj = Instantiate(randomObj, new Vector3(0f, 0f, 0f), Quaternion.identity);
            newObj.SetActive(false); 
            pickupPool.Add(newObj);
        }
        
        if (pickupPool.Count == poolSize && pickupPool[0] != null)
        {
            pickupPoolInitialised = true;
        }
    }
    
    GameObject[] foundPickup = new GameObject[1];

    // get a random pickup pickup from pool
    public GameObject GetPickupFromPool()
    {
        lock (ListLocker)
        {
            // setup list of objects to spawn
            if (!pickupPoolInitialised)
            {
                //pickupPoolInitialised = true;
                SetupPickupPool();
            }

            // Get a random element from pickup pool
            int listCount = pickupPool.Count;
            int random = Random.Range(0, listCount - 1);

            
            pickupPool.CopyTo(random, foundPickup, 0,1); // from pos,to array,

            // now remove from pickup list (this code is horrible - why is Microsoft so rubbish!)
            pickupPool.RemoveAt(random);
            foundPickup[0].SetActive(true);
            GameObject returnedObj = foundPickup[0];
            foundPickup[0] = null;
            return returnedObj;
        }
    }

    GameObject returnedPickup;

    // returns collected pickup to pool
    // do i need to implement try catch  block and start async operation to add object when it can
    public void ReturnPickupToPool(GameObject pickup) 
    {
        bool bPickupReturned = false;

        lock (ListLocker)
        {
            // reset transform position
            pickup.transform.position = new Vector3(0f, 0f, 0f);
            pickupPool.Add(pickup);
            pickup.SetActive(false);
            bPickupReturned = true;

            
        }

        if (!bPickupReturned)
        {
            // start async to return it - prob not necessary for this game!
            returnedPickup = pickup;
        }
    }
}
