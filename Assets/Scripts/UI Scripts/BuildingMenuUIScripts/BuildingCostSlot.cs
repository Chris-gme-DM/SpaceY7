using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingCostSlot : MonoBehaviour
{
    public void Setup(Resources costData, bool hasEnough)
    {
        TextMeshProUGUI amountText = GetComponentInChildren<TextMeshProUGUI>();
        Image resourceIcon = GetComponentInChildren<Image>();

        resourceIcon.sprite = costData.ResourceIcon;
        resourceIcon.color = hasEnough ? Color.white : Color.red;

        amountText.text = costData.amount.ToString();
        amountText.color = hasEnough ? Color.green : Color.red;
    }
}
