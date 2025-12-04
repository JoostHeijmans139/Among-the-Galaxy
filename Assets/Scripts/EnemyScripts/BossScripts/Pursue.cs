using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;

public class Pursue : State
{
    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PURSUE;
        agent.speed = 8;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        anim.SetFloat("blend", 1f);
        base.Enter();
    }

    public override void Update()
    {
        if (Vector3.Distance(npc.transform.position, player.position) > visDist)
        {
            nextState = new Idle(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
