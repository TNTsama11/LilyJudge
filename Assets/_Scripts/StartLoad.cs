using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using LitJson;
using System;

public class StartLoad : MonoBehaviour
{
    public Button startButton;
    public GameObject InputNamePanel;
    public Button enterBtn;
    public Button cancelBtn;
    public InputField inputField;

    void Awake()
    {

    }

    void Start()
    {
        startButton.onClick.AddListener(OnStartBtnClick);
        enterBtn.onClick.AddListener(OnEnterBtnClick);
        cancelBtn.onClick.AddListener(OnCancelBtnClick);
    }

    private void OnCancelBtnClick()
    {
        inputField.text = string.Empty;
        InputNamePanel.SetActive(false);
    }

    private void OnEnterBtnClick()
    {
        if (inputField.text == string.Empty)
        {
            return;
        }
        if (inputField.text != PlayerPrefs.GetString("PName"))
        {
            PlayerPrefs.SetInt("Num", 0);
            PlayerPrefs.SetString("PName", inputField.text);
        }
        GameManager.Instance.curName = inputField.text;
        GameManager.Instance.LoadScene();
    }

    private void OnStartBtnClick()
    {
        InputNamePanel.SetActive(true);
    }
          
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    

   
}
