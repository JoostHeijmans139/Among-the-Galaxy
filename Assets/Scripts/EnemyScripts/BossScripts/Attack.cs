using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;

public class Attack : State
{
    [SerializeField] private PlayerStats _playerStats;

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
        agent.SetDestination(npc.transform.position);
    }

    public override void Update()
    {
        if (Vector3.Distance(npc.transform.position, player.position) > attackDist)
        {
            //animation event
            anim.SetBool("attacking", false);
            nextState = new Pursue(npc, agent, anim, player);
            stage = EVENT.EXIT;

            PlayerStats.instance.TakeDamage(10f);
            _playerStats.TakeDamage(10f);
        }
        else
        {
            npc.transform.LookAt(new Vector3(player.position.x, npc.transform.position.y, player.position.z));
            agent.SetDestination(npc.transform.position);
            // attack method here
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
    public void Damage()
    {
        Debug.Log("Damage dealt to player");
    }

}
