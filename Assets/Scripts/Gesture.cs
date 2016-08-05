using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gesture
{
    /// <summary>
    /// Данные тачей
    /// </summary>
    public List<Vector3> mouseData = new List<Vector3>();

    /// <summary>
    /// Толщина линии
    /// </summary>
    public int PointWidth = 3;

    /// <summary>
    /// Последняя текстура
    /// </summary>
    public Texture2D LastPattern;

    /// <summary>
    /// Получить нарисованую текстуру
    /// </summary>
    /// <returns>The pattern.</returns>
    public Texture2D MapPattern()
    {
        //Првоерка количества
        if (this.mouseData.Count < 10)
        {
            Debug.Log("Error: Mouse count less 10");
            return null;
        }
            
        //Местность
        Bounds bounds = new Bounds(this.mouseData [0], Vector3.zero);
        for (int index = 1; index < this.mouseData.Count; ++index)
            bounds.SetMinMax(Vector3.Min(bounds.min, this.mouseData [index]), Vector3.Max(bounds.max, this.mouseData [index]));

        //Создать пустую текстуру
        LastPattern = new Texture2D(32, 32);
        Color[] pixels = LastPattern.GetPixels();

        //ЗАполнить белой текстурой
        for (int index = 0; index < pixels.Length; ++index)
            pixels [index] = Color.white;
            
        //Пройтись по всем точкам
        for (int index1 = 0; index1 < this.mouseData.Count - 1; ++index1)
        {
            int num1 = (int)Mathf.Clamp((float)(((double)this.mouseData [index1].x - (double)bounds.min.x) / (double)bounds.size.x * 32.0), 0.0f, 31f);
            int num2 = (int)Mathf.Clamp((float)(((double)this.mouseData [index1].y - (double)bounds.min.y) / (double)bounds.size.y * 32.0), 0.0f, 31f);
            int num3 = (int)Mathf.Clamp((float)(((double)this.mouseData [index1 + 1].x - (double)bounds.min.x) / (double)bounds.size.x * 32.0), 0.0f, 31f);
            int num4 = (int)Mathf.Clamp((float)(((double)this.mouseData [index1 + 1].y - (double)bounds.min.y) / (double)bounds.size.y * 32.0), 0.0f, 31f);

            float num5 = Mathf.Sqrt(Mathf.Pow((float)(num3 - num1), 2f) + Mathf.Pow((float)(num4 - num2), 2f));

            for (int index2 = 0; index2 <= 20; ++index2)
            {
                float num6 = (float)index2 * 0.05f;
                int num7 = (int)((double)num1 + (double)(num3 - num1) * (double)num6);
                int num8 = (int)((double)num2 + (double)(num4 - num2) * (double)num6);

                pixels [num7 + num8 * 32] = Color.black;

                for (int index3 = 1; index3 < this.PointWidth; ++index3)
                {
                    int num9 = (int)((double)num7 + (double)((num4 - num2) * index3) / (double)num5);
                    int num10 = (int)((double)num8 - (double)((num3 - num1) * index3) / (double)num5);
                    int num11 = (int)((double)num7 - (double)((num4 - num2) * index3) / (double)num5);
                    int num12 = (int)((double)num8 + (double)((num3 - num1) * index3) / (double)num5);

                    if (num9 >= 0 && num9 < 32 && (num10 >= 0 && num10 < 32))
                        pixels [num9 + num10 * 32] = Color.black;
                        
                    if (num11 >= 0 && num11 < 32 && (num12 >= 0 && num12 < 32))
                        pixels [num11 + num12 * 32] = Color.black;
                }
            }
        }

        //Установить
        LastPattern.SetPixels(pixels);
        LastPattern.Apply();

        //Вернуть
        return LastPattern;
    }

    /// <summary>
    /// Алгоритм 1
    /// </summary>
    /// <returns>The pattern.</returns>
    /// <param name="fromTexture">From texture.</param>
    /// <param name="toTexture">To texture.</param>
    public float Pattern_1(Texture2D fromTexture, Texture2D toTexture)
    {
        if (toTexture == null)
        {
            Debug.Log("Mouse Gesture Interpretation: texture pattern for comparison is not set.");
            return 1.0f;
        }

        Color[] pixels1 = fromTexture.GetPixels();
        Color[] pixels2 = toTexture.GetPixels();

        float num1 = 0.0f;
        float num2 = 0.0f;
        float num3 = 0.0f;
        for (int index = 0; index < pixels2.Length; ++index)
        {
            if (pixels2 [index] == Color.black)
                ++num1;
        }

        for (int index = 0; index < pixels1.Length; ++index)
        {
            if (pixels1 [index] == Color.black)
            {
                ++num2;
                if (pixels2 [index] == Color.black)
                    ++num3;
            }
        }
        float num4 = num2 - num3;
        float num5 = num3 / num1;
        if ((double)num4 < (double)num3)
            return 1.0f - num5;
        
        return 1.0f;
    }

    /// <summary>
    /// Второй алгоритм
    /// </summary>
    /// <returns>The 2.</returns>
    /// <param name="_original">Original.</param>
    /// <param name="_gesture">Gesture.</param>
    public float Pattern_2(Texture2D _original, Texture2D _gesture)
    {
        if(_gesture == null) return 1.0f;

        float diff = 0;

        //Пройтись по текстуре
        for (int y = 0; y < _original.height; y++)
        {
            for (int x = 0; x < _original.width; x++)
            {
                if(_gesture.GetPixel(x, y) != _original.GetPixel(x, y))
                    diff += 1.0f;
            }
        }

        return diff / (_original.width * _original.height);
    }
}