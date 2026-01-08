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

    public int sightRange = 20;
    public int attackRange = 4;

    public bool isInAttackRange = false;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        currentState = new Idle(this.gameObject, agent, anim, player);
    }

    void Update()
    {
        currentState = currentState.Process();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
        {
            isInAttackRange = true;
        }
        else
        {
            isInAttackRange = false;
        }
    }
    public void Damage()
    {
        if (isInAttackRange == true)
        {
            PlayerStats.instance.Health -= 10f;
            Debug.Log("Damage dealt to player");
        }
    }
}
