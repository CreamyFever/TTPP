using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

using TTPP;

public class JsonTestWindow : EditorWindow
{
    [MenuItem("JSON/JsonUtilityTest")]
    static void Init()
    {
        JsonTestWindow window = (JsonTestWindow)GetWindow(typeof(JsonTestWindow));
        window.Show();
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Save"))
        {
            Save();
        }
        if(GUILayout.Button("Load"))
        {
            Load();
        }
    }

    void Save()
    {
        PlayerInfo[] playerInfo = new PlayerInfo[3];

        for(int i = 0; i < playerInfo.Length; i++)
        {
            playerInfo[i] = new PlayerInfo();
        }
        
        playerInfo[0].name = "Mettaton";
        playerInfo[0].healthPoint = 100.0f;
        playerInfo[0].attackPoint = 15.0f;
        playerInfo[0].shieldPoint = 20.0f;
        playerInfo[0].score = 0.0f;

        playerInfo[1].name = "Prisk";
        playerInfo[1].healthPoint = 999.0f;
        playerInfo[1].attackPoint = 999.0f;
        playerInfo[1].shieldPoint = 999.0f;
        playerInfo[1].score = 100.0f;

        playerInfo[2].name = "Sans";
        playerInfo[2].healthPoint = 1000.0f;
        playerInfo[2].attackPoint = 1500.0f;
        playerInfo[2].shieldPoint = 2000.0f;
        playerInfo[2].score = 1000.0f;

        string toJson = JsonHelper.ArrayToJson(playerInfo);
        File.WriteAllText(Application.dataPath + "/Resources/Data/data.json", toJson);
        Debug.Log("Success!");
    }

    void Load()
    {
        Debug.Log(Application.dataPath);
        string jsonString = File.ReadAllText(Application.dataPath + "/Resources/Data/data.json");
        var data = JsonHelper.JsonToArray<PlayerInfo>(jsonString);

        foreach(var info in data)
        {
            Debug.Log(info.name);
            Debug.Log(info.healthPoint);
            Debug.Log(info.attackPoint);
            Debug.Log(info.shieldPoint);
            Debug.Log(info.score);
        }
    }
}
