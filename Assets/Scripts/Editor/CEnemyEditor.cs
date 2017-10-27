using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

using TTPP;

public class CEnemyEditor : EditorWindow
{
    [MenuItem("JSON/JSON Enemy Editor",false,12)]
    private static void OpenJsonEnemyEditor()
    {
        Initialize();
    }

    private static CEnemyEditor window;
    int width = 150;
    int widthSmall = 40;
    int height = 16;

    int spaceX = 80;
    int spaceY = 18;

    float contentWidth = 0.0f;
    float contentHeight = 0.0f;

    Vector2 scrollPos;

    [Range(1,9999)]
    int inputLevel = 1;

    

    public static void Initialize()
    {
        window = (CEnemyEditor)GetWindow(typeof(CEnemyEditor));
        window.minSize = new Vector2(500, 300);
    }

    private bool IsPlaying()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Cannot edit while game is being played.", MessageType.Warning);
            return true;
        }
        return false;
    }

    public void OnGUI()
    {
        float startX = 5.0f;
        float startY = 5.0f;

        if (IsPlaying())
            return;

        if (window == null)
            Initialize();

        startY = DrawGeneralSetting(startX, startY) + 5.0f;

        Rect visibleRect = new Rect(startX, startY + 15, window.position.width - startX * 2, window.position.height - 50);
        Rect contentRect = new Rect(startX, startY, contentWidth - 20, contentHeight+100);

        GUI.color = new Color(0.85f, 0.85f, 0.85f, 1.0f);
        GUI.Box(visibleRect, "");

        GUI.color = Color.white;

        scrollPos = GUI.BeginScrollView(visibleRect, scrollPos, contentRect);

        if (inputLevel != 0 && inputLevel <= CGameManager.Instance.LevelDataList.Count)
        {
            startY = DrawWaveList(startX, startY + 5, CGameManager.Instance.LevelDataList[inputLevel - 1]);
        }

        contentWidth = (subWaveBoxWidth + 10) * maxEnemySize + 20;

        contentHeight = startY - visibleRect.y - spaceY * 0.5f;

        GUI.EndScrollView();
    }

    float DrawGeneralSetting(float startX, float startY)
    {
        EditorGUI.LabelField(new Rect(startX, startY, width, height), new GUIContent("Level : "));
        inputLevel = EditorGUI.IntField(new Rect(startX + spaceX, startY, widthSmall, height), inputLevel);

        if (GUI.Button(new Rect(window.position.width - 130, startY, width - 40, height + 15), "Update"))
            UpdateFile();

        if (GUI.Button(new Rect(window.position.width - 260, startY, width - 40, height + 15), "Load"))
            CJsonFileManager.Instance.LoadLevelData();

        return startY + spaceY;
    }

    /// <summary>
    /// LevelDataをロードして敵の情報の部分だけ上書きする。
    /// </summary>
    void UpdateFile()
    {
        string jsonString = File.ReadAllText(Application.dataPath + "/Resources/Data/LevelData.json");
        var levelData = JsonHelper.JsonToArray<LevelData>(jsonString);

        LevelData data = levelData[inputLevel - 1];

        if (data != null)
        {
            for(int i = 0; i < data.waveList.Count; i++)
            {
                data.waveList[i].enemyList = CGameManager.Instance.LevelDataList[inputLevel - 1].waveList[i].enemyList;
            }
            
            string toJson = JsonHelper.ArrayToJson(levelData);
            File.WriteAllText(Application.dataPath + "/Resources/Data/LevelData.json", toJson);
        }
        else
            Debug.LogError("Failed to load!");
    }


    public int removeIndex = -1;
    public int maxEnemySize = 1;
    public List<bool> waveFoldList = new List<bool>();

    float DrawWaveList(float startX, float startY, LevelData data)
    {
        maxEnemySize = 1;
        for (int i = 0; i < data.waveList.Count; i++)
        {
            if (waveFoldList.Count <= i) waveFoldList.Add(false);

            startY += spaceY;

            waveFoldList[i] = EditorGUI.Foldout(new Rect(startX, startY, widthSmall * 2, 15), waveFoldList[i], "Wave - " + (i + 1).ToString());
            

            TTPP.PanelWave wave = data.waveList[i];

            if (waveFoldList[i])
            {
                startX += 15;

                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), new GUIContent("Enemies "));

                EditorGUI.IntField(new Rect(startX + spaceX, startY, widthSmall, height), wave.enemyList.Count);

                if (GUI.Button(new Rect(startX + spaceX + 40, startY, widthSmall, height), "-1"))
                {
                    if (wave.enemyList.Count > 1)
                        wave.enemyList.RemoveAt(wave.enemyList.Count - 1);
                }
                if (GUI.Button(new Rect(startX + spaceX + 80, startY, widthSmall, height), "+1"))
                {
                    if(wave.enemyList.Count < 4)
                        wave.enemyList.Add(new EnemyInfo());
                }

                float cachedY = startY + spaceY;

                for (int j = 0; j < wave.enemyList.Count; j++)
                {
                    startY = DrawEnemyBox(startX + (j * subWaveBoxWidth + 10), cachedY, wave.enemyList[j]);
                }
                
                CGameManager.Instance.LevelDataList[inputLevel - 1].waveList[i].enemyList = wave.enemyList;
                //startY += spaceY * 2.0f;

                startX -= 15;
            }

            maxEnemySize = Mathf.Max(waveFoldList[i] ? wave.enemyList.Count : 1, maxEnemySize);
        }

        return startY + spaceY;
    }


    private float subWaveBoxWidth = 0;
    private float subWaveBoxHeight = 0;
    float DrawEnemyBox(float startX, float startY, EnemyInfo enemy)
    {
        float spaceX = 60;
        float cachedY = startY;
        width -= 10;
        
        subWaveBoxWidth = spaceX + width + 5;
        GUI.Box(new Rect(startX, startY, subWaveBoxWidth, subWaveBoxHeight), "");

        startX += 5;
        startY += 3;
        
        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "Prefab:");
        enemy.prefab = (GameObject)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, widthSmall * 2, height),
            enemy.prefab, typeof(GameObject), false);

        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "Sprite: ");
        enemy.sprite = (Sprite)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, widthSmall * 2, height),
            enemy.sprite, typeof(Sprite), false);

        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "Name: ");
        enemy.name = EditorGUI.TextField(new Rect(startX + spaceX, startY, widthSmall * 2, height),
            enemy.name);

        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "LeftTurn: ");
        enemy.leftTurn = EditorGUI.IntField(new Rect(startX + spaceX, startY, widthSmall, height),
            enemy.leftTurn);
        
        startY += 10;

        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "SP: ");
        enemy.shieldPoint = EditorGUI.FloatField(new Rect(startX + spaceX, startY, widthSmall, height), enemy.shieldPoint);
        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "HP: ");
        enemy.healthPoint = EditorGUI.FloatField(new Rect(startX + spaceX, startY, widthSmall, height), enemy.healthPoint);
        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "ATK: ");
        enemy.attackPoint = EditorGUI.FloatField(new Rect(startX + spaceX, startY, widthSmall, height), enemy.attackPoint);


        width += 10;
        subWaveBoxHeight = startY - cachedY + spaceY + 2;


        return startY + spaceY;
    }
}
