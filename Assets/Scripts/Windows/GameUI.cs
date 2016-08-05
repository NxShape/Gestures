using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GameUI : IWindow<GameUI>
{
    /// <summary>
    /// Начата ли игра
    /// </summary>
    public bool GameStarted;
    
    /// <summary>
    /// Текст таймера
    /// </summary>
    public Text TimerText;

    /// <summary>
    /// Таймер
    /// </summary>
    public TimeSpan Timer;

    /// <summary>
    /// Текст задания
    /// </summary>
    public Text TaskText;

    /// <summary>
    /// Активный таск
    /// </summary>
    public int ActiveTask;

    /// <summary>
    /// Последнее задание
    /// </summary>
    public int LastTask;

    /// <summary>
    /// Количество законченых тасков
    /// </summary>
    public int CompletedTasks;

    /// <summary>
    /// Рендерер таска
    /// </summary>
    public LineRenderer TaskRenderer;

    /// <summary>
    /// Первая точка
    /// </summary>
    public Transform FirstPoint;

    /// <summary>
    /// Контроллер распознавания
    /// </summary>
    public Gesture GestureController;

    /// <summary>
    /// Если началась
    /// </summary>
    public bool IsDrag;

    /// <summary>
    /// Расстояние для регистрации точек
    /// </summary>
    public float PointDistance;

    /// <summary>
    /// Главная камера
    /// </summary>
    public Camera MainCamera;

    /// <summary>
    /// Пересчет позиции
    /// </summary>
    Vector3 VirtualPosition;

    /// <summary>
    /// Последняя используемая позиция мыши
    /// </summary>
    Vector3 LastPosition;

    /// <summary>
    /// Эффект
    /// </summary>
    public Transform Effect;

    /// <summary>
    /// Данные
    /// </summary>
    public List<GestureData> Data;

    /// <summary>
    /// ТЕкстура для рисования
    /// </summary>
    public RawImage NeedTexture;

    void Start()
    {
        Hide();
    }

    /// <summary>
    /// Событие по открыванию
    /// </summary>
    public override void OpenAction()
    {
        base.OpenAction();

        //Сбросить
        CompletedTasks = 0;
        SetTask(0);

        GameStarted = true;
    }

    /// <summary>
    /// Установка таска
    /// </summary>
    /// <param name="_number">Number.</param>
    void SetTask(int _number)
    {        
        ActiveTask = _number;
        Timer = new TimeSpan(0, 0, 10 - Mathf.RoundToInt(0.5f * CompletedTasks));

        //Установить задание
        TaskText.text = "Завершеные таски: " + CompletedTasks;

        //Текстура
        NeedTexture.texture = Data[ActiveTask].Pattern;
    }

    void Update()
    {
        //Проверка
        if(!GameStarted) return;

        if(Input.GetMouseButtonDown(0) & !IsDrag)
        {
            IsDrag = true;
        }

        //Если нажата кнопка
        if(Input.GetMouseButton(0) && IsDrag)
        {
            VirtualPosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            VirtualPosition.z = 0.0f;

            if(GestureController.mouseData.Count == 0 || (Vector3.Distance(LastPosition, Input.mousePosition) > PointDistance))
            {
                if(GestureController.mouseData.Count == 0)
                {
                    Effect.gameObject.SetActive(false);
                    Effect.position = VirtualPosition;
                    Effect.gameObject.SetActive(true);
                }
                
                GestureController.mouseData.Add(VirtualPosition);
                LastPosition = Input.mousePosition;

                TaskRenderer.SetVertexCount(GestureController.mouseData.Count);
                TaskRenderer.SetPosition(GestureController.mouseData.Count - 1, VirtualPosition);
            }

            Effect.position = new Vector3(VirtualPosition.x, VirtualPosition.y, 0.0f);
        }

        //Если был отжата кнопка
        if(Input.GetMouseButtonUp(0) && IsDrag)
        {
            IsDrag = false;

            float _percent = GestureController.Pattern_2(Data[ActiveTask].Pattern, GestureController.MapPattern());
            Debug.Log("Percent: " + _percent.ToString("f2"));

            GestureController.mouseData = new List<Vector3>();

            if(Data[ActiveTask].NeedPercent > _percent)
            {
                CompletedTasks++;

                //Yайти новый таск
                int _new_task = ActiveTask;
                while(_new_task == ActiveTask)
                    _new_task = UnityEngine.Random.Range(0, Data.Count);

                //Установить
                SetTask(_new_task);
            }

            TaskRenderer.SetVertexCount(0);
        }

        //Осмотр времени
        if(Timer.TotalMilliseconds > 0)
        {
            //Вывести на экран
            TimerText.text = Timer.Seconds.ToString("00") + "." + Timer.Milliseconds.ToString("000");

            //Уменьшить
            Timer = Timer.Subtract(new TimeSpan(0, 0, 0, (int)(Time.deltaTime), (int)(Time.deltaTime * 1000.0f)));
        }
        else
            GameOver();
    }

    /// <summary>
    /// Окончание игры
    /// </summary>
    public void GameOver()
    {
        GameStarted = false;

        //Перейти
        Hide();
        GameOverUI.Open();
    }
}
