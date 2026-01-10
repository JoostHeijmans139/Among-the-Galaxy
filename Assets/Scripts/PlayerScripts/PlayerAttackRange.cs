using UnityEngine;

public class PlayerAttackRange : MonoBehaviour
{
    public EnemyAI enemyInRange;

    private void OnTriggerEnter(Collider other)
    {
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemyInRange = enemy;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null && enemy == enemyInRange)
        {
            enemyInRange = null;
        }
    }
}
