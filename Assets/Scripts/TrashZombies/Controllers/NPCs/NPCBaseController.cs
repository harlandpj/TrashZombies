using System;
using System.Collections;
using System.Collections.Generic;
using TrashZombies.Pickups;
using UnityEngine;
using UnityEngine.AI;

// MODIFY TO USE A STATE MACHINE (if I have time - don't use bools)

/// <summary>
/// Abstract Base Class for Enemies
/// - All enemies have a list of patrol (walk) points as they will not be stationary UNLESS
/// there is a corresponding entry indicating they should treat the current point as a stationary point
/// (this is to be added later on in dev if time permits!)
/// </summary>
[RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(AudioSource))]
public abstract class NPCBaseController : MonoBehaviour
{
    protected Animator m_Animator; // animator component
    protected NavMeshAgent navAgent; // navigation mesh agent
    protected AudioSource audioSource; // audio source

    [SerializeField]
    protected GameObject Player; // Player character
    protected Transform playerTransform; // Player transform

    [SerializeField]
    protected GameObject rubbishPointOnNPC; // set in editor to point on skeleton where rubbish is thrown from

    [SerializeField]
    protected AudioClip attackNoise;

    [SerializeField]
    protected float m_Health; // current health

    [SerializeField]
    protected float maxHealth; // more than 100% for Boss Enemies

    // accessors
    public float Health
    {
        get => m_Health;

        protected set
        {
            if (value < maxHealth)
            {
                if (value <= 0)
                {
                    m_Health = 0;
                }
                else
                {
                    m_Health = value;
                }
            }
            else
            {
                m_Health = maxHealth;
            }
        }
    }

    protected int m_CurrentPatrolPoint = 0; // patrol point character currently at

    // set current patrol point
    public int CurrentPatrolPoint
    {
        get => m_CurrentPatrolPoint;

        protected set
        {
            if (value < 0)
            {
                // invalid, set to starting position
                m_CurrentPatrolPoint = 0;
            }
            else if (value > m_PatrolPoints.Length - 1)
            {
                // invalid, set to last patrol point
                m_CurrentPatrolPoint = m_PatrolPoints.Length - 1;
            }
            else
            {
                // anything else
                m_CurrentPatrolPoint = value;
            }
        }
    }

    protected GameObject m_DestinationPoint; // destination is either towards Player / next Patrol Point / or idling doing something
    protected int m_Strength; // some enemies are stronger than others (and will take longer to kill)
    protected int m_DamageDealt; // damage dealt to player when attacking
    protected float m_Speed; // walk speed of enemy
    protected float m_SprintSpeed; // sprint speed of enemy
    protected float m_EyesightDistance; // some enemies see the player (for raycast distance) further away

    // filled from the Character stats entry set in editor
    [SerializeField]
    protected CharacterStats characterStatistics;

    // NOTE: Uses (too many) bools for now - MUST change to a State Machine (if time allows)

    [Header("Enemy Current State")]
    [SerializeField]
    protected bool m_Attacking = false; // true if currently attacking player

    [SerializeField]
    protected bool m_Patrolling = true; // always starts off patrolling between points (unless overridden)

    [SerializeField]
    protected bool m_Reversing = false; // sets to true at end of patrol, resets to false when start position reached again

    protected string m_EnemyName; // name of enemy for UI display (later on in dev)

    [Space]
    [SerializeField]
    protected GameObject[] m_PatrolPoints; // array of patrol points

    // override in derived class for any specific requirements
    protected NPCBaseController()
    {
    }

    protected void Awake()
    {
        SetupEnemy();
    }

    // set to starting position (invisible gameObjects in scene are used to define points)
    public virtual void SetToFirstPatrolPosition()
    {
        CurrentPatrolPoint = 0;
        m_DestinationPoint = m_PatrolPoints[0];
    }

    protected virtual void Start()
    {
        // reset player to new character when allowing choice of characters (later on in dev)
        ChangePlayerReference();

        // setup animation events for Thrown object (load onto hand, and throw rubbish)
        AnimationClip throwAnimation;

        // set up event
        AnimationEvent throwRubbishEvent;
        AnimationEvent loadRubbishEvent;
        throwRubbishEvent = new AnimationEvent();
        loadRubbishEvent = new AnimationEvent();

        throwRubbishEvent.time = 0.5f;
        loadRubbishEvent.time = 0.1f;

        throwRubbishEvent.functionName = "ThrowRubbishAtPlayer";
        loadRubbishEvent.functionName = "LoadRubbishOntoLeftHand";

        Animator anim = GetComponent<Animator>();

        // find correct animation clip from animator
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Throw")
            {
                throwAnimation = clip;
            }
        }

        throwAnimation = anim.runtimeAnimatorController.animationClips[3];
        throwAnimation.AddEvent(throwRubbishEvent);
        throwAnimation.AddEvent(loadRubbishEvent);

        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // change Player used
    public virtual void ChangePlayerReference()
    {
        // functionality NOT USED YET!
        // allow changing the player character after selection from choices in the main menu
        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.transform;
    }

    // used later on in dev (not strictly necessary here!)
    public virtual void SetupDynamicPatrolPoint(int arrayNum, GameObject obj)
    {
        // set with dynamic object created in spawn manager
        m_PatrolPoints[arrayNum] = obj;
    }

    // override in derived class & always call base method
    public virtual void SetupEnemy()
    {
        // always call base in derived class

        navAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // defaults: these are customised in derived classes from the relevant CharacterStatistics data (setup in Unity Editor)
        Health = 100; // max health
        m_Speed = 5f; // default walk speed
        m_SprintSpeed = 11f; // default sprint speed
        m_DamageDealt = 5;  // default attack damage
        m_EyesightDistance = 30f; // default vision distance 

        navAgent.speed = m_Speed; // set speed of character

        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.GetComponent<Transform>();

        if (m_PatrolPoints != null)
        {
            // set initial destination to first patrol point
            CurrentPatrolPoint = 0;
            m_DestinationPoint = m_PatrolPoints[0];
        }
        else
        {
            // just go towards Player
            m_DestinationPoint = Player;
            Debug.LogError("NPCBaseController::SetupEnemy, No Patrol Points have been Setup in editor!");
        }
    }

    float attackDistance = 15f; // distance at which NPC should stop and attack (just throwing stuff in this game)
    bool bStartedAttack = false;

    // override in derived class to provide any specific attack functionality
    public virtual void AttackPlayer()
    {


        if (!m_Attacking)
        {
            m_Attacking = true; // added see if makes a difference!

            float distanceBetween = Vector3.Distance(transform.position,
                             Player.transform.position);

            if (distanceBetween <= attackDistance)
            {


                if (!bStartedAttack)
                {
                    Debug.Log($"Now in AttackPlayer - Started Attack!");
                    bStartedAttack = true;
                    SetStartAttacking();
                }
            }
            else
            {
                Debug.Log($"AttackPlayer - Outside Attack Distance!");
                bStartedAttack = false;

                if (distanceBetween < m_EyesightDistance)
                {
                    Debug.Log($"AttackPlayer - Set to Approaching!");
                    ResetToApproaching();
                }
                else
                {
                    Debug.Log($"AttackPlayer - Set to Patrolling!");
                    ResetToPatrolling();
                }
            }
        }

        RotateTowardsPlayer();
    }

    void SetStartAttacking()
    {
        //m_Attacking = true;
        m_Patrolling = false;
        m_Animator.SetBool("Attack", true);
        m_Animator.SetFloat("Speed", 0f);
        navAgent.destination = transform.position; // don't move!
        navAgent.speed = 0f;
    }

    [SerializeField]
    private float afterAttackPlayerDistance = 10f;

    void ResetToApproaching()
    {
        float distanceBetween = Vector3.Distance(transform.position,
                             Player.transform.position);

        if (distanceBetween <= afterAttackPlayerDistance)
        {
            // don't start running if only a short distance away
            m_Animator.SetFloat("Speed", m_Speed);
        }
        else
        {
            m_Animator.SetFloat("Speed", m_SprintSpeed);
        }

        m_Attacking = false;
        m_Animator.SetBool("Attack", false);
        m_Animator.SetFloat("Speed", m_SprintSpeed);
        navAgent.speed = m_SprintSpeed;
        navAgent.destination = Player.transform.position; // don't move!
        m_Patrolling = false;
    }

    void ResetToPatrolling()
    {
        m_Attacking = false;
        m_Animator.SetBool("Attack", false);
        m_Animator.SetFloat("Speed", m_Speed);
        navAgent.speed = m_Speed;
        m_Patrolling = true;
    }

    IEnumerator DontAddDamage()
    {
        // adds damage (throws something)
        yield return new WaitForSeconds(2f);
        m_Animator.SetBool("Attack", false);
        AddDamage();
    }

    protected virtual void StopAttacking()
    {
        m_Attacking = false;
        m_Animator.SetBool("Attack", false);
        m_Animator.SetFloat("Speed", m_Speed);
        navAgent.speed = m_Speed;
        stopMovingForNow = false;
    }

    // adds damage to health of enemy
    public virtual void AddDamage()
    {
        // die if health is zero, derived must call this base method
        AmIDead();
    }

    // move the enemy
    public virtual void MoveEnemy()
    {
        if (!GameController.Instance.m_bGameOver)
        {
            m_Animator.SetFloat("Speed", 1.5f);

            if (PlayerSeenOrInRange())
            {
                MoveTowardsPlayer();
            }
            else
            {
                Patrol();
            }
        }
    }

    protected bool PlayerSeenOrInRange()
    {
        // checks if player is visible, i.e. within eyesight range in plain view,
        // and not hidden behind something in scene
        if (Vector3.Distance(transform.position,
                             Player.transform.position) <= m_EyesightDistance)
        {
            float height = Math.Abs(transform.position.y - Player.transform.position.y);

            if (height < 1f)
            {
                // similar heights so can attack and player IS in range to be approached and attacked

                // *REMOVED FOR NOW*
                // Only attack if visible (this bit removed for now for simplicity, but WILL use in future!)
                // need to change to raycast in 3 forward directions (at -45,0,45 degrees) while moving to see if Player found

                //RaycastHit pointHit = new RaycastHit();

                //Vector3 direction = new Vector3(Player.transform.position.x - transform.position.x,
                //                                Player.transform.position.y - transform.position.y,
                //                                Player.transform.position.z - transform.position.z);
                //Ray shootRay = new Ray();

                //shootRay.direction = direction;
                //shootRay.origin = transform.position;

                // shoot a ray and see if we can hit Player (i.e. with range AND visible)
                //Vector3 shootPoint = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

                //if (Physics.Raycast(shootPoint, transform.forward, out pointHit, m_EyesightDistance))
                //if (Physics.Raycast(shootRay.direction, pointHit.transform.position, m_EyesightDistance))
                //{
                //// we hit something in range with the raycast
                //Debug.Log("Ray hit :  " + pointHit.transform.name);
                //Debug.Log("Position of Ray cast Hit (x,y,z) is: x=   " + pointHit.point.x + ", y=   " + pointHit.point.y + ", z=   " + pointHit.point.z + ".");
                //Debug.DrawRay(shootRay, transform.TransformDirection(Vector3.forward) * pointHit.distance, Color.white, 2.0f);
                //Debug.DrawRay(transform.position, shootRay.direction, Color.white, 2.0f);

                // check if Player
                //    if (pointHit.transform.CompareTag("Player"))
                //    {
                //        Debug.Log("Player is VISIBLE!");
                //        m_Patrolling = false; // starts move towards player
                //        return true;
                //    }
                //    else
                //    {
                //        m_Patrolling = true; // end move towards player
                //        return false;
                //    }
                //}
                //else
                //{
                //    m_Patrolling = true;
                //    return false;
                //}

                //m_Animator.SetFloat("Speed", 0); // stop and attack
                //m_Animator.SetBool("Attack", true);

                SetStartAttacking();
                //m_Patrolling = false;
                return true;
            }
            else
            {
                // Player may be standing above Enemy (on a box /tower etc) where there is no nav path

                //m_Patrolling = true;
                //m_Animator.SetFloat("Speed", m_Speed); // continue
                //m_Animator.SetBool("Attack", false);
                //ResetToApproaching();
                return false;
            }
        }

        return false;
    }

    protected bool bMakeApproachNoise = false;
    protected bool bPlayingApproach = false;
    protected bool bPlayingAttack = false;
    protected bool stopMovingForNow = false;

    protected void MoveTowardsPlayer()
    {

        if (!stopMovingForNow)
        {
            // just chase the player
            if (navAgent != null)
            {
                navAgent.destination = playerTransform.position;
            }
        }

        // check if in attack range
        CloseEnoughToAttack();
    }

    protected void RotateTowardsPlayer()
    {
        // rotate enemy to face player
        transform.LookAt(Player.transform);
    }

    private void CloseEnoughToAttack()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) <= attackDistance)
        {
            // in attack range, stop moving when attacking
            stopMovingForNow = true;

            // set navAgent destination to be my current transform to avoid repeated movement
            // towards player during attack

            if (navAgent != null)
            {
                navAgent.SetDestination(transform.position);
                navAgent.speed = 0f;
            }

            //if (!attacking player)
            Debug.Log("Stopped, now Attacking Player!");
            AttackPlayer();
        }
        else
        {
            // stop attack and restart movement
            StopAttacking();

            if (navAgent != null)
            {
                navAgent.SetDestination(Player.transform.position);
                navAgent.speed = m_Speed;
            }

            stopMovingForNow = false;
            m_Attacking = false;
        }
    }

    // enemy death
    protected virtual void Die()
    {
        AmIDead();
    }

    protected void AmIDead()
    {
        // checks if have now died
        if (Health <= 0)
        {
            //DisableHits();
            Death();
        }
    }

    private void DisableHits()
    {
        // disable hits on whatever type of collider object has
        if (gameObject.GetComponent<CapsuleCollider>() != null)
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }

        if (gameObject.GetComponent<MeshCollider>() != null)
        {
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }

        if (gameObject.GetComponent<BoxCollider>() != null)
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }

    }

    private void Death()
    {
        // destroy with slight delay (use a Pool Manager in a longer project to avoid GC overheads)
        PlayDeathSound();
    }

    protected virtual void PlayDeathSound()
    {
        // start a coroutine to kill enemy after 
        // playing any animations / sounds etc
    }

    // never called
    protected virtual IEnumerator PlayDeath()
    {
        yield return null;
    }

    // enemy patrol function
    public virtual void Patrol()
    {
        if (m_PatrolPoints != null)
        {
            // check if we have any patrol points set
            if (m_PatrolPoints.Length == 0)
            {
                // this enemy just goes towards the player
                if (navAgent != null)
                {
                    navAgent.destination = Player.transform.position;
                }
            }
            else
            {
                // just patrol from our current position to next patrol point
                // reversing at end to go back to original position
                if (m_DestinationPoint == null)
                {
                    Debug.Log("Destination doesnt exist! Setting to Player Transform: " + playerTransform.position);
                    m_DestinationPoint = Player;
                }

                if (Vector3.Distance(transform.position,
                                     m_DestinationPoint.transform.position) <= 1.5f)
                {
                    // check if we were reversing and reached beginning
                    if (m_Reversing)
                    {
                        if (CurrentPatrolPoint == 0)
                        {
                            // we are at start of patrol
                            m_Reversing = false;
                            CurrentPatrolPoint++;
                        }
                        else
                        {
                            // go to next point
                            CurrentPatrolPoint--;
                        }
                    }
                    else
                    {
                        // not currently reversing, check have reached end of patrol
                        if (CurrentPatrolPoint >= m_PatrolPoints.Length - 1)
                        {
                            // go in reverse
                            m_Reversing = true;
                            CurrentPatrolPoint--;
                        }
                        else
                        {
                            // continue on to next patrol point
                            CurrentPatrolPoint++;
                        }
                    }

                    // set next destination
                    m_DestinationPoint = m_PatrolPoints[CurrentPatrolPoint];

                    if (navAgent != null)
                    {
                        navAgent.destination = m_DestinationPoint.transform.position;
                    }
                }
                else
                {
                    // set destination
                    if (navAgent != null)
                    {
                        navAgent.destination = m_DestinationPoint.transform.position;
                    }
                }
            }
        }
        else
        {
            // no patrol points set - enemy just goes towards the player
            if (navAgent != null)
            {
                navAgent.destination = Player.transform.position;
            }
        }
    }

    // will make a noise specific to the type of enemy
    protected virtual void MakeAttackNoise()
    {
        // implement different noises in derived classes
        StartCoroutine(PlayingAttack());
    }

    protected virtual IEnumerator PlayingAttack()
    {
        // must have something here as can't override audioClips!
        yield return new WaitForSeconds(0.1f);
    }

    protected virtual void StopPlayingAttack() { }

    GameObject currentRubbishObject;
    bool bRubbishLoadedOntoHand;

    protected void LoadRubbishOntoLeftHand()
    {
        // get some rubbish from Pool first time round, add to NPC skeleton and throw it towards player later
        //if (!bRubbishLoadedOntoHand)
        //{
        //    GameObject rubbishObject = PickupPoolManager.Instance.GetPickupFromPool();

        //    // place on hand in skeleton - working BUT won't appear for some reason!
        //    rubbishObject.transform.parent = rubbishPointOnNPC.transform;
        //    rubbishObject.transform.position = rubbishPointOnNPC.transform.position;

        //    currentRubbishObject = rubbishObject;
        //    //gameObject.SetActive(true);
            Debug.Log("In LoadRubbishOntoLeftHand! - COMMENTED OUT NOW");
        //}
    }

    /// <summary>
    /// Throw Rubbish at Player - Instantiates a random one
    /// </summary>
    public void ThrowRubbishAtPlayer()
    {
        // ok the above isn't working - so just throw an object towards player
        // and parent all individual items capable of being thrown onto skeleton and
        // set active the correct one (to display it) - depending on what I am throwing, UNTIL it's thrown!
        Debug.Log("Throw Rubbish Event called by Player Object!");

        Player = GameObject.FindGameObjectWithTag("Player");

        if (Math.Abs(transform.position.y - Player.transform.position.y) <2f)
        {
            // only throw when player at a similar level, not when in a building or wherever
            Vector3 throwDirection = Player.transform.position - transform.position;

            // instantiate a new one
            GameObject[] pickups = PickupPoolManager.Instance.pickups;

            int randomOne = 0; // set to zero as others not working properly yet! Should ALWAYS be wine bottle thrown!

            Vector3 newPos = new Vector3(rubbishPointOnNPC.transform.position.x,
                    rubbishPointOnNPC.transform.position.y,
                    rubbishPointOnNPC.transform.position.z);

            GameObject throwThis = Instantiate(pickups[randomOne], newPos, Quaternion.identity);

            throwThis.SetActive(true);
            throwThis.GetComponent<PickupBase>().SetThrownByNPC(true);
            throwThis.GetComponent<PickupBase>().hitByPlayer = false; // just in case
            throwThis.GetComponent<Rigidbody>().useGravity = true;

            // allow some variance to stop multiple thrown objects sticking together on screen
            throwDirection = Player.transform.position + 
                new Vector3(0 + UnityEngine.Random.Range(0, 0.01f), 
                0.9f + UnityEngine.Random.Range(0,0.3f), UnityEngine.Random.Range(0, 0.01f))
                - throwThis.transform.position;

            throwThis.GetComponent<Rigidbody>().AddForce(throwDirection * 45, ForceMode.VelocityChange);
        }
    }
}
