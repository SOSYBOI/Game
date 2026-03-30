using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{

    [SerializeField]
    private Button[] buttons;

    // Start is called before the first frame update
    private void Start()
    {
        if (buttons == null) return;
        buttons[0].onClick.AddListener(CloseGame);
        buttons[1].onClick.AddListener(ClosePanel);  
    }

 
    private void CloseGame() => Application.Quit();

    private void ClosePanel() => gameObject.SetActive(false);

}
