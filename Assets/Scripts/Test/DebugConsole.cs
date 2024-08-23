using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    uint qsize = 15;  // number of messages to keep
    Queue myLogQueue = new Queue();

    GUIStyle logStyle;

    Vector2 scrollPosition;

    public int fontSize = 30;


    private void Start()
    {
        Debug.Log("Started up logging");
        DontDestroyOnLoad(gameObject);

        logStyle = new GUIStyle();
        logStyle.fontSize = fontSize; // Set font size
        logStyle.normal.textColor = Color.black;
        logStyle.wordWrap = true;
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        //myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();

        string color = "black";
        string colorNomalLog = "black";
        string colorWarmingLog = "yellow";
        string colorErrorLog = "red";

        if (type == LogType.Log) color = colorNomalLog;
        if(type == LogType.Warning || type == LogType.Exception) color = colorWarmingLog;
        if (type == LogType.Error || type == LogType.Assert) color = colorErrorLog;

        myLogQueue.Enqueue($"<color={color}>[{type}] : {logString}</color>");
        if (type == LogType.Exception)
            myLogQueue.Enqueue($"<color={color}>{stackTrace}</color>");

        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    void OnGUI()
    {
        //GUILayout.BeginArea(new Rect(100, 0, Screen.width/2, Screen.height-200));
        //GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()), logStyle);
        //GUILayout.EndArea();

        //scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width / 2), GUILayout.Height(Screen.height - 200));
        //GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()), logStyle);
        //GUILayout.EndScrollView();

        float logAreaWidth = Screen.width / 2;
        float logAreaHeight = Screen.height / 3;
        GUILayout.BeginArea(new Rect(0, Screen.height - logAreaHeight, logAreaWidth, logAreaHeight));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(logAreaWidth), GUILayout.Height(logAreaHeight));
        GUILayout.Label(string.Join("\n", myLogQueue.ToArray()), logStyle);
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}
