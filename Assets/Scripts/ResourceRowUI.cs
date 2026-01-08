using UnityEngine;
using TMPro;

public class ResourceRowUI : MonoBehaviour
{
    public TMP_Text resourceNameText;
    public TMP_Text resourceAmountText;

    public void Set(ResourceCost cost)
    {
        int owned = PlayerStats.Instance.GetAmount(cost.resourceName);

        resourceNameText.text = cost.resourceName;
        resourceAmountText.text = $"{owned} / {cost.amount}";
        resourceAmountText.color = owned >= cost.amount ? Color.white : Color.red;
    }
}
