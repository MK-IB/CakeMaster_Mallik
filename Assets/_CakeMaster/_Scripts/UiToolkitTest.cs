using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UiToolkitTest : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    private void Start()
    {
        VisualElement root = _uiDocument.rootVisualElement;
        Button button1 = root.Q<Button>("button1");
        Button button2 = root.Q<Button>("button2");
        Button button3 = root.Q<Button>("button3");

        button1.clicked += Button1Clicked;
        button2.clicked += Button2Clicked;
        button3.clicked += Button3Clicked;
    }

    void Button1Clicked()
    {
        Debug.Log("Button-1-Clicked");
    }
    void Button2Clicked()
    {
        Debug.Log("Button-2-Clicked");
    }
    void Button3Clicked()
    {
     Debug.Log("Button-3-Clicked");   
    }
}
