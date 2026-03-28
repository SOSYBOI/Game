using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeybindUI : MonoBehaviour,IPointerClickHandler
{
    [SerializeField]
    private TextMeshProUGUI keybindText;
    [SerializeField]
    private string commandName;
    private bool bindSelected = false;

    private void Update()
    {
        if (bindSelected)
        {
            foreach(KeyCode key in Enum.GetValues(typeof(KeyCode))){
                if (Input.GetKeyDown(key))
                {
                    if (KeyBindManager.Instance.IsSameKey(commandName, key))
                    {
                        DisableBinding();
                    }
                    else if (KeyBindManager.Instance.IsValidKey(commandName,key))
                    {
                        keybindText.text = key.ToString();
                        DisableBinding();
                    }
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!KeyBindManager.IsActive)
        {
            EnableBinding();
        }else if (bindSelected)
        {
            DisableBinding();
        }
    }

    private void EnableBinding()
    {
        KeyBindManager.Instance.OpenPanel();
        KeyBindManager.IsActive = true;
        bindSelected = true;
    }

    private void DisableBinding()
    {
        KeyBindManager.Instance.ClosePanel();
        KeyBindManager.IsActive = false;
        bindSelected = false;
    }

}
