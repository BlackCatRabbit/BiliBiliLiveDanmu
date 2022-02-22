using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputUIController : MonoBehaviour
{
    public GameObject InputPanel;
    public string text;
    public GameObject ScoketController;
    public static InputUIController _instance;
    public InputField inputField;
    private void Awake()
    {
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text = inputField.text;

    }

    public void OnClickEnter()
    {

        ScoketController.SetActive(true);
        InputPanel.SetActive(false);
    }
}
