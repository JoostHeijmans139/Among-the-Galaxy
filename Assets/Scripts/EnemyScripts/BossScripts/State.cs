using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class State
{
    //Create states
    public enum STATE
    {
        IDLE, PATROL, PURSUE, ATTACK, IDLE2
    };

    //Create events
    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    //State variables
    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    protected Transform player;
    protected State nextState;
    protected NavMeshAgent agent;

    //Detection variables
    public float visDist = 20.0f;
    public float visAngle = 30.0f;
    public float attackDist = 4.0f;

    //State constructor
    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    {
        npc = _npc;
        agent = _agent;
        anim = _anim;
        stage = EVENT.ENTER;
        player = _player;
    }

    //Keep updating states
    public virtual void Enter()
    {
        stage = EVENT.UPDATE;
    }
    public virtual void Update()
    {
        stage = EVENT.UPDATE;
    }
    public virtual void Exit()
    {
        stage = EVENT.EXIT;
    }

    //Process state changes
    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }
}