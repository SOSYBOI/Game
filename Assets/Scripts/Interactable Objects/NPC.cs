using cherrydev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField]
    private DialogNodeGraph _graph;

    [SerializeField]
    private string characterName;

    public static bool IsInteracting = false;
    private bool _currentlyInteracting = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_currentlyInteracting) return;
        
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            DialogManager.Instance.StartDialog(_graph);
            DialogManager.Instance.CloseMessage();
            _currentlyInteracting = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsInteracting) return;

        if (other.CompareTag("Player"))
        {
            IsInteracting = true;
            _currentlyInteracting = true;
            DialogManager.Instance.OpenMessage($"Chat to {characterName} (F)");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_currentlyInteracting) return;

        if (other.CompareTag("Player"))
        {
            IsInteracting = false;
            _currentlyInteracting = false;
            DialogManager.Instance.CloseMessage();
        }
    }

}
