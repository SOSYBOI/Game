using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipSystem : MonoBehaviour
{
    public static ToolTipSystem Instance;

    [SerializeField]
    private ToolTipUI toolTipUI;

    [SerializeField]
    private Vector2 mouseOffset;

    private bool uiSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        CloseToolTip();
    }

    // Update is called once per frame
    void Update()
    {
        if (uiSelected)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector2 newOffset = mouseOffset;
            Vector3 tooltipPosition = mousePosition + (Vector3)mouseOffset;

            Debug.Log(Screen.width);
            Debug.Log(tooltipPosition.x + 300);
            Debug.Log(tooltipPosition.x - 300);

            Debug.Log(Screen.height);
            Debug.Log(tooltipPosition.y + 130);
            Debug.Log(tooltipPosition.y - 130);

            if (tooltipPosition.x+300 > Screen.width || tooltipPosition.x-300 < 0) {
                newOffset.x = -mouseOffset.x;
            }

            if (tooltipPosition.y + 130 > Screen.height || tooltipPosition.y - 130 < 0)
            {
                newOffset.y = -mouseOffset.y;
            }

            toolTipUI.GetComponent<RectTransform>().position = mousePosition + (Vector3)newOffset;
        }
    }

    public void OnToolTipSelected(EquipmentSlotUI ui)
    {
        if (toolTipUI != null)
        {
            if (ui.EquippedItem == null)
            {
                Debug.Log("Equipment selected is null.");
                return;
            }
            toolTipUI.gameObject.SetActive(true);
            toolTipUI.SetDescription(ui.EquippedItem.ItemName,ui.EquippedItem.ItemDescription);
            uiSelected = true;
        }
    }

    public void OnToolTipSelected(SkillNodeUI ui)
    {
        if (toolTipUI != null)
        {
            if(ui.SkillNode == null)
            {
                Debug.Log("Skill Node cannot be null!");
                return;
            }else if (ui.SkillNode.SkillData == null)
            {
                Debug.Log("Skill Data cannot be null!");
            }

            toolTipUI.gameObject.SetActive(true);
            toolTipUI.SetDescription(ui.SkillNode.SkillData.UpgradeName,ui.SkillNode.SkillData.Value.ToString());
            uiSelected = true;
        }
    }

    public void CloseToolTip()
    {
        toolTipUI.gameObject.SetActive(false);
        uiSelected = false;
    }
}
