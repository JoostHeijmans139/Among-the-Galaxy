using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;
using JetBrains.Annotations;

public class Pursue : State
{
    //State constructor
    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PURSUE;
        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = 5;
            agent.isStopped = false;
        }
    }

    //Enter pursue state
    //Set animation and destination
    public override void Enter()
    {
        anim.SetFloat("blend", 1f);
        base.Enter();
        agent.SetDestination(npc.transform.position);
    }

    //Update pursue state
    //Check for player in vision range
    //If out of vision range, switch to idle state
    //If in attack range, switch to attack state
    //Else, pursue player
    public override void Update()
    {
        if (player == null) return;
        
        if (Vector3.Distance(npc.transform.position, player.position) > visDist)
        {
            nextState = new Idle(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
        else if (Vector3.Distance(npc.transform.position, player.position) < attackDist)
        {
            nextState = new Attack(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
        else
        {
            if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking01"))
            {
                agent.SetDestination(npc.transform.position);
            }
            else
            {
                agent.SetDestination(player.position);
            }
        }
    }

    //Exit pursue state
    public override void Exit()
    {
        base.Exit();
    }
}
