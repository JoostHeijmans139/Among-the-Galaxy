using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;
public class Patrol : State
{
    int currentIndex = -1;

    //State constructor
    public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PATROL;
        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = 2;
            agent.isStopped = false;
        }
    }

    //Enter patrol state
    //Set animation and destination
    public override void Enter()
    {
        currentIndex = 0;
        anim.SetFloat("blend", 0.5f);
        base.Enter();
        agent.SetDestination(GameEnvironment.Singleton.Checkpoints[currentIndex].transform.position);
    }

    //Update patrol state
    //Check for player in vision range
    //If in vision range, switch to pursue state
    //Else, move to next checkpoint
    public override void Update()
    {
        if (player == null) return;
        
        if (Vector3.Distance(npc.transform.position, player.position) > visDist)
        {
            if (agent.remainingDistance < 1)
            {
                if (currentIndex >= GameEnvironment.Singleton.Checkpoints.Count - 1)
                    currentIndex = 0;
                else
                    currentIndex++;

                agent.SetDestination(GameEnvironment.Singleton.Checkpoints[currentIndex].transform.position);
            }   
        }
        else
        {
            nextState = new Pursue(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    //Exit patrol state
    public override void Exit()
    {
        base.Exit();
    }
}
