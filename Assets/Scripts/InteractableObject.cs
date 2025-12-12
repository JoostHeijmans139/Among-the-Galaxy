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

    public string GetItemName()
    {
        return ObjectName;
    }

    public void OnPunchOrShoot()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(Shake());

        ObjectHealth--;
        if (ObjectHealth < 0)
        {
            Debug.Log($"Item {ItemName} added to inventory");
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
