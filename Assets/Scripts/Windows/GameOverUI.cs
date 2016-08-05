using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverUI : IWindow<GameOverUI>
{
    /// <summary>
    /// Описание
    /// </summary>
    public Text TaskText;
    
    void Start()
    {
        Hide();
    }

    public override void OpenAction()
    {
        base.OpenAction();

        //Установить задание
        TaskText.text = "Завершеные таски: " + GameUI.Instance.CompletedTasks;
    }

    /// <summary>
    /// Рестарт игры
    /// </summary>
    public void RestartGame()
    {
        Hide();
        GameUI.Open();
    }
}
