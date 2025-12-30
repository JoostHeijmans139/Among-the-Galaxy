using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string ObjectName;
    public string ItemName;
    public int ObjectHealth = 3;
    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    // Amount of materials given on hit
    public int hitGiveAmount = 2;

    // Amount of materials given on destruction
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

        // Give player resource
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.GainResource(ItemName, hitGiveAmount);
        }

        // Reduce object health
        ObjectHealth--;

        // Check if object should die
        if (ObjectHealth < 0)
        {
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.GainResource(ItemName, destroyGiveAmount);
                Debug.Log($"Item {ItemName} added to inventory");
            }

            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

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
