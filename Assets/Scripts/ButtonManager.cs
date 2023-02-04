using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.instance.StartGame();
    }

    public void ShowInfo()
    {
        GameManager.instance.LoadInfoScene();
    }

    public void LoadMenu()
    {
        GameManager.instance.MenuScene();
    }

    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }
}
