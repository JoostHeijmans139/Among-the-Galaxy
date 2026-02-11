using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEditor;

public class Idle : State
{
    //State constructor
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) 
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.IDLE;
        if (!agent.IsUnityNull())
        {
            if (agent.isOnNavMesh)
            {
                SetupNavMeshAgent(0, true);
            }
            else
            {
                if (!npc.IsUnityNull())
                {
                    MonoBehaviour mb = npc.GetComponent<MonoBehaviour>();
                    if (!mb.IsUnityNull())
                    {
                        mb.StartCoroutine(WaitForNavMesh(() => SetupNavMeshAgent(0, true)));
                    }
                }
            }
        }
    }

    //Enter idle state
    //Set animation and destination
    public override void Enter()
    {
        anim.SetFloat("blend", 0f);
        base.Enter();
        agent.SetDestination(npc.transform.position);
    }

    //Update idle state
    //Check for player in vision range
    //If in vision range, switch to pursue state
    //Randomly switch to patrol state
    public override void Update()
    {
        if (player == null) return;
        
        if (Vector3.Distance(npc.transform.position, player.position) < visDist)
        {
            nextState = new Pursue(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
        else if (Vector3.Distance(npc.transform.position, player.position) < attackDist)
        {
            nextState = new Attack(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
        else
        {
            if (Random.Range(0, 10000) < 10)
            {
                nextState = new Patrol(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }
        }
    }

    //Exit idle state
    public override void Exit()
    {
        base.Exit();
    }
}
