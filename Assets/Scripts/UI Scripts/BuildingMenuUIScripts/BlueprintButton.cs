using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
/// <summary>
/// This script is attached to the Button Prefab of the Building Menu UI/Blueprint List and dynamically selects the correct building data.
/// </summary>
public class BlueprintButton : MonoBehaviour
{
    [SerializeField] private Image m_previewImage;
    [SerializeField] private TextMeshProUGUI m_buildingNameText;
    private Button m_button;

    private BuildingData m_buildingData;
    private BuildingMenuUI m_menuUI;
    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(OnClicked);
    }

    public void Setup(BuildingData data, BuildingMenuUI UIManager)
    {
        m_buildingData = data;
        m_menuUI = UIManager;
        m_previewImage.sprite = data.PreviewImage;
        m_buildingNameText.text = data.BuildingName;
    }

    private void OnClicked()
    {
        BuildingMenuUI.Instance.DisplayBlueprint(m_buildingData);
    }
}
