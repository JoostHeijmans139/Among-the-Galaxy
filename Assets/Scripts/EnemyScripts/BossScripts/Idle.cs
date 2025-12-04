using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;

public class Idle : State
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) 
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.IDLE;
        agent.speed = 0;
        agent.isStopped = true;
    }

    public override void Enter()
    {
        anim.SetFloat("blend", 0f);
        base.Enter();
        agent.SetDestination(npc.transform.position);
    }

    public override void Update()
    {
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

    public override void Exit()
    {
        base.Exit();
    }
}
