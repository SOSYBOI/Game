using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBindManager Instance;

    public static bool IsActive = false;

    public static UnityAction OnKeybindSelected;

    private Dictionary<string, KeyCode> keys;

    [SerializeField]
    private GameObject chooseKeyPanel;
    [SerializeField]
    private TextMeshProUGUI debugText;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        keys = new Dictionary<string, KeyCode>();
        keys.Add("Up",KeyCode.W);
        keys.Add("Left", KeyCode.A);
        keys.Add("Right", KeyCode.D);
        keys.Add("Down", KeyCode.S);
        ClosePanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPanel()
    {
        chooseKeyPanel.SetActive(true);
        debugText.text = "Press Any Key.";
    }

    public void ClosePanel()
    {
        chooseKeyPanel.SetActive(false);
    }

    public bool IsSameKey(string commandName, KeyCode key)
    {
        return keys[commandName].Equals(key);
    }

    public bool IsValidKey(string commandName, KeyCode key)
    {
        string message = string.Empty;
        if(key == KeyCode.Escape)
        {
            message = "Escape Key cannot be binded.";
            debugText.text = message;
            return false;
        }

        string cmdName = keys.FirstOrDefault(x => x.Value == key).Key;
        if (cmdName != null && cmdName != commandName)
        {
            message = key.ToString()+" has already been used by "+cmdName+".";
            debugText.text = message;
            return false;
        }
        return true;

    }

}
