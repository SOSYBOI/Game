using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipSystem : MonoBehaviour
{
    public static ToolTipSystem Instance;

    [SerializeField]
    private ToolTipUI prefab;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnToolTipSelected(EquipmentUI ui)
    {
        if (prefab != null)
        {
            prefab.gameObject.SetActive(true);
            prefab.SetDescription("");
        }
    }

    public void OnToolTipSelected(SkillNodeUI ui)
    {
        if (prefab != null)
        {
            prefab.gameObject.SetActive(true);
            prefab.SetDescription("");
        }
    }

    public void CloseToolTip()
    {
        prefab.gameObject.SetActive(false);
    }
}
