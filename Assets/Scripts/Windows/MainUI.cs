using UnityEngine;
using System.Collections;

public class MainUI : IWindow<MainUI>
{
    void Start()
    {
        Open();
    }

    /// <summary>
    /// Начать игру
    /// </summary>
    public void StartGame()
    {
        Hide();
        GameUI.Open();
    }
}
