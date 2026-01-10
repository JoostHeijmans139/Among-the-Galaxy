using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDrop
{
    public string itemName;

    // 0 = 0%, 1 = 100%
    [Range(0f, 1f)] public float dropChance = 1f; 
    public int amount = 1;
}

public class InteractableObject : MonoBehaviour
{
    // Name shown in UI when looking at object
    public string ObjectName;

    // If object does not use chance system it gives this item
    public string ItemName;
    public int ObjectHealth = 3;
    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    // Multiple items that can drop on hit with chances
    public List<ItemDrop> hitDrops = new List<ItemDrop>();

    // Amount of materials given on hit without chance system
    public int hitGiveAmount = 2;

    public int destroyGiveAmount = 5; 

    public string GetItemName()
    {
        return ObjectName;
    }

    public void OnPunchOrShoot()
    {
        // Stop previous shake
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(Shake());

        // Give player resources on hit
        if (PlayerStats.Instance != null)
        {
            // Use new multi-drop system if configured
            if (hitDrops != null && hitDrops.Count > 0)
            {
                foreach (ItemDrop drop in hitDrops)
                {
                    // Roll for each item based on drop chance
                    if (Random.value <= drop.dropChance)
                    {
                        PlayerStats.Instance.GainResource(drop.itemName, drop.amount);
                        Debug.Log($"Dropped {drop.amount}x {drop.itemName} (chance: {drop.dropChance * 100}%)");
                    }
                }
            }
            else if (!string.IsNullOrEmpty(ItemName))
            {
                // Fallback to legacy single item system
                PlayerStats.Instance.GainResource(ItemName, hitGiveAmount);
            }
        }

        // Reduce object health
        ObjectHealth--;

        // Check if object should die
        if (ObjectHealth < 0)
        {
            // On destruction, only give legacy destroy bonus if ItemName is set
            if (PlayerStats.Instance != null && !string.IsNullOrEmpty(ItemName))
            {
                PlayerStats.Instance.GainResource(ItemName, destroyGiveAmount);
                Debug.Log($"Destroyed! Bonus: {destroyGiveAmount}x {ItemName}");
            }

            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    // Shake animation for hitting object
    private IEnumerator Shake()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 randomPoint = originalPosition + Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = randomPoint;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}
