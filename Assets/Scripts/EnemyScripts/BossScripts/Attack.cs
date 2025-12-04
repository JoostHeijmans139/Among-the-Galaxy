using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;

public class Attack : State
{
    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.ATTACK;
        agent.speed = 0;
        agent.isStopped = true;
    }

    public override void Enter()
    {
        anim.SetBool("attacking", true);
        base.Enter();
    }

    public override void Update()
    {
        if (Vector3.Distance(npc.transform.position, player.position) > attackDist)
        {
            anim.SetBool("attacking", false);
            nextState = new Pursue(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
        else
        {
            npc.transform.LookAt(new Vector3(player.position.x, npc.transform.position.y, player.position.z));
            // attack method here
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
