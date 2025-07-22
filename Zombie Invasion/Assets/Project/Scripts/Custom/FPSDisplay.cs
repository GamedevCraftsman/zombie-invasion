using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    float fps = 0.0f;
    float updateInterval = 0.5f; // Час між оновленнями в секундах
    float timeSinceLastUpdate = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        timeSinceLastUpdate += Time.unscaledDeltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            fps = 1.0f / deltaTime;
            timeSinceLastUpdate = 0.0f;
        }
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(10, 10, w, h * 2 / 100f);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 75;
        style.normal.textColor = Color.red;
        string text = $"FPS: {fps:0}";
        GUI.Label(rect, text, style);
    }
}