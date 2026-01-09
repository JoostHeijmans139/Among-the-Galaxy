using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }

    NavMeshAgent agent;
    Animator anim;
    public Transform player;
    State currentState;

    //Adding range
    public int sightRange = 20;
    public int attackRange = 4;

    public bool isInAttackRange = false;

    public float health = 50f;

    private void Awake()
    {
        Instance = this;
    }

    //On start, create new state on the object
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        currentState = new Idle(this.gameObject, agent, anim, player);
    }

    //Handle state
    void Update()
    {
        currentState = currentState.Process();
        UpdateHealth();
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

    //Deal damage
    public void Damage()
    {
        if (isInAttackRange == true)
        {
            PlayerStats.Instance.Health -= 10f;
            Debug.Log("Damage dealt to player");
        }
    }

    //Destroy enemy on 0 health
    private void UpdateHealth()
    {
        if (health <= 0f)
        {
            Destroy(this.gameObject);
            Debug.Log("Enemy defeated");
        }
    }
}
