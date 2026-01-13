using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration;
using UnityEngine.AI;
using UnityEditor;

public class EnemyAI : MonoBehaviour
{

    NavMeshAgent agent;
    Animator anim;
    public Transform player;
    State currentState;

    private static GameObject _gameOverscreen;
    private static TMPro.TMP_Text _timeText;
    public Canvas _gameOverscreenNonStatic;
    public TMPro.TMP_Text _timeTextNonStatic;
    //Adding range
    public int sightRange = 20;
    public int attackRange = 4;
    public bool isInAttackRange = false;

    public float health = 50f;

    public int materialDropAmount = 5;
    
    public float timeSurived = 0f;

    //On start, create new state on the object
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        // if(_gameOverscreenNonStatic != null && _timeTextNonStatic != null)
        // {
        //     _gameOverscreen = _gameOverscreenNonStatic;
        //     _timeText = _timeTextNonStatic;
        // }
        // else
        // {
        //     Debug.Log("Game over screen or time text not assigned in inspector.");
        // }
        _gameOverscreen = GameObject.FindGameObjectWithTag("GameOverScreen");
        if (_gameOverscreen != null)
        {
            _gameOverscreen.SetActive(false);
        }
        // _timeText = GameObject.FindGameObjectWithTag("TimeSurvivedText").GetComponent<TMPro.TMP_Text>();
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError($"[EnemyAI] Player not found for enemy {gameObject.name}. Make sure the player has the 'Player' tag.");
            }
        }
        Debug.Log(AsyncNavMeshBuildScheduler.isNavMeshBaked);
        // Validate NavMeshAgent is on NavMesh
        if (agent != null && !agent.isOnNavMesh && AsyncNavMeshBuildScheduler.isNavMeshBaked)
        {
            Debug.LogWarning($"[EnemyAI] {gameObject.name} is not on a NavMesh yet. Attempting to warp to NavMesh...");
            // Try to warp to nearest NavMesh position
            UnityEngine.AI.NavMeshHit navHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out navHit, 10f, UnityEngine.AI.NavMesh.AllAreas))
            {
                agent.Warp(navHit.position);
                Debug.Log($"[EnemyAI] {gameObject.name} warped to NavMesh at {navHit.position}");
            }
            else
            {
                Debug.LogError($"[EnemyAI] {gameObject.name} could not find nearby NavMesh within 10 units!");
            }
        }
        
        currentState = new Idle(this.gameObject, agent, anim, player);
    }

    //Handle state
    void Update()
    {
        if (currentState != null)
        {
            currentState = currentState.Process();
        }
        timeSurived += Time.deltaTime;
    }

    //Give visuals to the different ranges
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    //Checks if player is in attack range
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInAttackRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInAttackRange = false;
        }
    }

    public void Damage()
    {
        if (isInAttackRange == true)
        {
            PlayerStats.Instance.Health -= 10f;
            Debug.Log("Damage dealt to player");
        }
    }

    //Take damage
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health >= 0f)
        {
            _gameOverscreen.SetActive(true);
            Time.timeScale = 0;
            UiHelper.SetTimeSurvived(timeSurived, _timeText);
            Cursor.lockState = CursorLockMode.None;
        }
        UpdateHealth();
    }

    //Destroy enemy on 0 health
    private void UpdateHealth()
    {
        if (health <= 0f)
        {
            PlayerStats.Instance.GainResource("Stone", materialDropAmount);
            Debug.Log($"Player has {PlayerStats.Instance.Stone} stone");

            Destroy(gameObject);
            Debug.Log("Enemy defeated");
        }
    }
}
