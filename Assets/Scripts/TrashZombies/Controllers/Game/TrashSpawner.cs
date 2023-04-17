using System.Collections;
using System.Collections.Generic;
using TrashZombies;
using UnityEngine;

/// This is a SINGLETON class.
/// </summary>
public class TrashSpawner : MonoBehaviour
{
    //[Header]
    public GameObject[] pickups = new GameObject[5]; // drag gameobjects in using editor to be instantiated later

    //[SerializeField]
    //int poolSize = 1000;  // default to 1000 objects - but can be changed in editor

    private bool trashSpawnerInitialised = false;

    /// <summary>
    /// Instance function for access to instance
    /// </summary>
    public static TrashSpawner Instance { get; private set; }

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
        DontDestroyOnLoad(gameObject);
    }

    //int trashSpawnInterval = 5; // FOR TESTING
    //int numToSpawnThisLevel = 20; // FOR TESTING

    int trashSpawnInterval = 60; // seconds (more balanced)
    int numToSpawnThisLevel = 10;
    
    //int levelNumber; // add later don't have time now!

    // Start is called before the first frame update
    void Start()
    {
        // setup list of objects to spawn
        if (!trashSpawnerInitialised)
        {
            trashSpawnerInitialised = true;
            InvokeRepeating("StartTrashSpawning", 5f, trashSpawnInterval); 
            StartTrashSpawning(); // start 1st spawn in 40 secs time
        }
    }

    

    // starts trash spawning into scene
    void StartTrashSpawning()
    {
        for (int numToSpawn = 0; numToSpawn < numToSpawnThisLevel; numToSpawn++)
        {
            int randomOne = UnityEngine.Random.Range(0, pickups.Length - 1);
            
            int randomSpawnArea = Random.Range(0,4); // needs to be 4 as hardly ever gets to 3 otherwise

            int randomX1 = Random.Range(400, 470); // player start positon area
            int randomX2 = Random.Range(430, 445); // central road
            int randomX3 = Random.Range(280, 500); // last road across bottom

            int randomY = Random.Range(135, 140); // height at which spawns won't drop thru ground mesh!

            int randomZ1 = Random.Range(65, 125); // player start positon area
            int randomZ2 = Random.Range(65, 350); // central road to end by sign
            int randomZ3 = Random.Range(265, 360); // last road at end by sign

            // spawn in front of original player start position or down central road
            Vector3 newPos;

            switch (randomOne)
            {
                case 1:
                    {
                        newPos = new Vector3(randomX1, randomY, randomZ1);
                        break;
                    }

                case 2:
                    {
                        newPos = new Vector3(randomX2, randomY, randomZ2);
                        break;
                    }

                case 3:
                    {
                        newPos = new Vector3(randomX3, randomY, randomZ3);
                        break;
                    }

                default:
                    {
                        newPos = new Vector3(randomX1, randomY, randomZ1);
                        break;
                    }
            }

            // rotate to correct upright position
            Quaternion originalRotation = Quaternion.Euler(
                -90f,
                pickups[randomOne].transform.rotation.y,
                pickups[randomOne].transform.rotation.z);

            GameObject dropThis = Instantiate(pickups[randomOne], newPos, Quaternion.identity);
            dropThis.transform.rotation = originalRotation;

            dropThis.SetActive(true);
            dropThis.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
