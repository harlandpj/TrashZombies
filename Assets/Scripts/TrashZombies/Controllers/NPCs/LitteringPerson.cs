using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Littering Person class - A male or female character that will attack the Player
/// </summary>
public class LitteringPerson : NPCBaseController
{
    [Header("Littering Person Noises")]
    [SerializeField]
    private AudioClip approachNoise;

    // not used yet!
    //[SerializeField]
    //private AudioClip littererAttackNoise;

    //[SerializeField]
    //private AudioClip littererDeath;

    //[SerializeField]
    //private AudioClip littererHitNoise;

    [SerializeField]
    public CharacterStats LitteringPersonStats; // modify in inspector!

    public LitteringPerson()
    {
        m_EnemyName = "Litterer";
    }

    protected override void Awake()
    {
        base.Awake();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public override void SetToFirstPatrolPosition()
    {
        base.SetToFirstPatrolPosition();
    }

    public override void SetupEnemy()
    {
        base.SetupEnemy(); // call this first

        // set initial values from assigned (in editor) character statistics entry
        m_Speed = LitteringPersonStats.NormalSpeed;
        m_SprintSpeed= LitteringPersonStats.SprintSpeed;
        m_DamageDealt = LitteringPersonStats.AttackDamage;
        m_EyesightDistance = LitteringPersonStats.EyesightDistance;
        maxHealth = LitteringPersonStats.MaxHealth;
        m_Health = LitteringPersonStats.MaxHealth; // initially same as max health
        m_EnemyName = LitteringPersonStats.CharName;

        navAgent.speed = m_Speed; // normal speed
        m_Animator = GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
        m_Attacking = false;

        // find patrol points
        if (m_PatrolPoints[0] == null)
        {
            Debug.LogError("LitteringPerson::SetupEnemy - No Patrol Points set, using Player as destination");
        }
    }

    public override void AddDamage()
    {
        // Adds damage to enemy from Player
        Health -= LitteringPersonStats.AttackDamage;

        base.AddDamage(); // this checks if now dead
        Debug.Log("Litterer Health is now: " + Health);
    }

    public override void AttackPlayer()
    {
        base.AttackPlayer();
    }

    protected override void MakeAttackNoise()
    {
        Debug.Log($"Now in LitteringPerson::MakeAttackNoise: m_Attacking = {m_Attacking}");
    }

    protected override IEnumerator PlayingAttack()
    {
        while (m_Attacking)
        {
            if (!bPlayingAttack)
            {
                // attack player
                bPlayingAttack = true;
                m_Animator.SetFloat("Speed", 0f);
                m_Animator.SetBool("Throw", true);
                navAgent.speed = 0f; // stop moving
            }

            yield return new WaitForSeconds(2.5f); // add damage every 2.5 seconds
        }

        if (!m_Attacking)
        {
            StopPlayingAttack();
            m_Animator.SetBool("Throw", false);
            m_Animator.SetFloat("Speed", m_Speed);
            navAgent.speed = m_Speed; // normal speed
         
            yield break; // stop coroutine
        }

        yield break;
    }

    protected override void StopPlayingAttack()
    {
        base.StopPlayingAttack();

        if (GameController.Instance.m_bGameOver)
        {
            StopCoroutine("PlayingAttack");
        }
    }

    public override void Patrol()
    {
        base.Patrol(); // added - but may not be necessary in this example project
    }

    public override void MoveEnemy()
    {
        // enemies should ALWAYS be moving (or doing something if not patrolling)
        base.MoveEnemy();
    }

    private void FixedUpdate()
    {
        if (!GameController.Instance.m_bGameOver)
        {
            // move the enemy (either patrolling or idling around)
            MoveEnemy();
        }
        else
        {
            StopPlayingAttack();
        }
    }

    protected override void Start()
    {
        base.Start(); // sets up animation event for Thrown Rubbish

        m_Animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        SetupEnemy();
        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.GetComponent<Transform>();
    }

    protected override void PlayDeathSound()
    {
        // litterer is dead
        Debug.Log("Litterer has Died");

        //audioSource.PlayOneShot(littererDeath, 1f) ;
        GameController.Score += 100;
        //GameController.Instance.RemoveAnEnemy();
        Destroy(gameObject, 1f);
    }

    public override void SetupDynamicPatrolPoint(int arrayNum, GameObject obj)
    {
        base.SetupDynamicPatrolPoint(arrayNum, obj);
    }
}
