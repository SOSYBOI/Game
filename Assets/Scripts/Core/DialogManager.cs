using cherrydev;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is used to manage the dialogs.
/// The dialog graphs are from NPC class, please look at the class member _dialogGraph.
/// 
/// In addition, you can also add function if you reach to a specific dialog. 
/// To do this, first you need add the function to this script. 
/// Then you need to add the following code in Start function,
/// _dialogBehaviour.BindExternalFunction("Test",Test); 
/// (The string name does not need to be the same as the actual function name but for clarification, just do it like this for now.)
/// Finally, in that DialogNodeGraph you made, you can either add external function attribute and add your function string name from one of the existing Sentence Nodes,
/// OR you can add a external function node.
/// 
/// I have made an example under and also made a DialogNodeGraph in the NPC Dialog Graphs folder.
/// You create DialogNodeGraph by going to Assets/Create/ScriptableObjects/NodeGraph/NodeGraph
/// You can also create Variable Config for uses of DialogNodeGraph by going to Assets/Create/ScriptableObjects/VariableConfig
/// 
/// With this, you can manage variable in DialogNodeGraph.
/// </summary>
public class DialogManager : MonoBehaviour
{

    public static DialogManager Instance;

    public static UnityAction OnDialogStarted;

    [SerializeField]
    private DialogBehaviour _dialogBehaviour;

    [SerializeField]
    private TextMeshProUGUI openInteractUI;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        _dialogBehaviour.BindExternalFunction("Test",Test);
    }

    /// <summary>
    /// This is called when player interacted with a NPC object.
    /// </summary>
    /// <param name="graph"></param>
    public void StartDialog(DialogNodeGraph graph)
    {
        _dialogBehaviour.StartDialog(graph);
    }

    public void OpenMessage(string text)
    {
        openInteractUI.text = text;
        openInteractUI.gameObject.SetActive(true);
    }

    public void CloseMessage()
    {
        openInteractUI.gameObject.SetActive(false);
    }

    //Place all the External function below.
    private void Test()
    {
        Debug.Log("External function has been invoked.");
    }

}
