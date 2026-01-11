using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;

public class EnemyAI : MonoBehaviour
{

    NavMeshAgent agent;
    Animator anim;
    public Transform player;
    State currentState;

    //Adding range
    public int sightRange = 20;
    public int attackRange = 4;
    public bool isInAttackRange = false;

    public float health = 50f;

    public int materialDropAmount = 5;

    //On start, create new state on the object
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError($"[EnemyAI] Player not found for enemy {gameObject.name}. Make sure the player has the 'Player' tag.");
            }
        }
        
        // Validate NavMeshAgent is on NavMesh
        if (agent != null && !agent.isOnNavMesh)
        {
            Debug.LogWarning($"[EnemyAI] {gameObject.name} is not on a NavMesh yet. Attempting to warp to NavMesh...");
            // Try to warp to nearest NavMesh position
            UnityEngine.AI.NavMeshHit navHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out navHit, 10f, UnityEngine.AI.NavMesh.AllAreas))
            {
                agent.Warp(navHit.position);
                Debug.Log($"[EnemyAI] {gameObject.name} warped to NavMesh at {navHit.position}");
            }
            else
            {
                Debug.LogError($"[EnemyAI] {gameObject.name} could not find nearby NavMesh within 10 units!");
            }
        }
        
        currentState = new Idle(this.gameObject, agent, anim, player);
    }

    //Handle state
    void Update()
    {
        if (currentState != null)
        {
            currentState = currentState.Process();
        }
    }

    //Give visuals to the different ranges
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    //Checks if player is in attack range
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInAttackRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInAttackRange = false;
        }
    }

    public void Damage()
    {
        if (isInAttackRange == true)
        {
            PlayerStats.Instance.Health -= 10f;
            Debug.Log("Damage dealt to player");
        }
    }

    //Take damage
    public void TakeDamage(float amount)
    {
        health -= amount;
        UpdateHealth();
    }

    //Destroy enemy on 0 health
    private void UpdateHealth()
    {
        if (health <= 0f)
        {
            PlayerStats.Instance.GainResource("Stone", materialDropAmount);
            Debug.Log($"Player has {PlayerStats.Instance.Stone} stone");

            Destroy(gameObject);
            Debug.Log("Enemy defeated");
        }
    }
}
