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
        anim.SetFloat("blend", 0f);
        base.Enter();
    }

    public override void Update()
    {
        //
    }

    public override void Exit()
    {
        base.Exit();
    }
}
