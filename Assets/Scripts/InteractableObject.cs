using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string ItemName;

    public string GetItemName()
    {
        return ItemName;
    }

    public void OnPunchOrShoot()
    {
        Debug.Log($"Item {ItemName} added to inventory");
        Destroy(gameObject);
    }
}
