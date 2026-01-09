using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;

public class Attack : State
{
    //State constructor
    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.ATTACK;
        agent.speed = 0;
        agent.isStopped = true;
    }

    //Enter attack state
    //Set animation and destination
    public override void Enter()
    {
        anim.SetBool("attacking", true);
        base.Enter();
        agent.SetDestination(npc.transform.position);
    }

    //Update attack state
    //Check for player in attack range
    //If out of attack range, switch to pursue state
    //Else, face player
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
            agent.SetDestination(npc.transform.position);
        }
    }

    //Exit attack state
    public override void Exit()
    {
        base.Exit();
    }
}
