using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{
    public int health;
    public int damage;
    public Material defaultEntityMat;
    Material entityMat;
    public MeshRenderer[] meshRenderers;

    // Audio
    public AudioClip[] hurtSounds;
    public AudioClip[] saySounds;
    public AudioClip[] stepSounds;
    public AudioClip deathSound;

    public AudioSource audioSource;

    bool audioPlaying;

    public NavMeshAgent agent;

    public PlayerHealth player;

    public LayerMask groundMask, playerMask;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    public float sightRange, attackRange, patrolRange, despawnRange;
    public bool playerInSightRange, playerInAttackRange, playerInPatrolRange, playerInsideTotalRange;
    bool dead = false;
    float tParam = 0f;
    private void Awake()
    {
        entityMat = new Material(defaultEntityMat);
        StartCoroutine(Init());
        player = FindAnyObjectByType<PlayerHealth>();
        agent = GetComponent<NavMeshAgent>();
    }
    IEnumerator Init()
    {
        foreach(var renderer in meshRenderers)
        {
            renderer.material = entityMat;

            yield return null;
        }
    }
    private void Update()
    {
        if (dead)
        {
            agent.enabled = false;
            Die();
        }
        else
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
            playerInPatrolRange = Physics.CheckSphere(transform.position, patrolRange, playerMask);
            playerInsideTotalRange = Physics.CheckSphere(transform.position, despawnRange, playerMask);

            if (agent.velocity != Vector3.zero && !audioPlaying)
            {
                StartCoroutine(PlayStepSounds());
            }
            if (!playerInsideTotalRange)
            {
                Despawn();
            }
            if (!playerInSightRange && !playerInAttackRange && !playerInPatrolRange && playerInsideTotalRange)
            {
                return;
            }
            else
            {
                if (!playerInSightRange && !playerInAttackRange && playerInPatrolRange && playerInsideTotalRange)
                {
                    Patrolling();
                }
                if (playerInPatrolRange && playerInSightRange && !playerInAttackRange && playerInsideTotalRange)
                {
                    ChasePlayer();
                }
                if (playerInPatrolRange && playerInAttackRange && playerInSightRange && playerInsideTotalRange)
                {
                    AttackPlayer();
                }
            }
        }
    }
    IEnumerator PlayStepSounds()
    {
        audioPlaying = true;
        audioSource.clip = stepSounds[Random.Range(0, stepSounds.Length)];
        audioSource.Play();
        yield return new WaitForSeconds(1f);
        audioPlaying= false;
    }
    void Patrolling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundMask))
        {
            walkPointSet = true;
        }
    }
    void ChasePlayer()
    {
        agent.SetDestination(player.transform.position);
    }
    void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player.transform, transform.up);

        if (!alreadyAttacked)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(damage);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    void ResetAttack()
    {
        alreadyAttacked = false;
    }
    public void TakeDamage(int damage)
    {
        if (dead)
        {
            return;
        }
        if ((health - damage) <= 0)
        {
            dead = true;
            audioSource.clip = deathSound;
            audioSource.Play();
        }
        else
        {
            health -= damage;

            audioSource.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
            audioSource.Play();
        }
        StartCoroutine(Damage());
    }
    IEnumerator Damage()
    {
        while (tParam < 1)
        {
            tParam += Time.deltaTime * 4f;
            if (dead)
            {
                entityMat.color = Color.Lerp(Color.white, Color.red, tParam);
            }
            else
            {
                if (tParam < 0.5)
                {
                    entityMat.color = Color.Lerp(Color.white, Color.red, tParam * 2);
                }
                else
                {
                    entityMat.color = Color.Lerp(Color.red, Color.white, tParam * 2);
                }
            }
            
            yield return new WaitForEndOfFrame();
        }
        tParam = 0;
    }
    void Die()
    {
        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 90f), 6f * Time.deltaTime);
        if (transform.localEulerAngles.z >= 89)
        {
            Invoke(nameof(Despawn), 1f);
        }
    }
    void Despawn()
    {
        GameObject.Destroy(this.gameObject);
    }
}
