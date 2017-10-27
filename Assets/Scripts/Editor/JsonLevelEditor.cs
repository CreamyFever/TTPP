using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

using TTPP;

public class JsonLevelEditor : EditorWindow
{
    [MenuItem("JSON/JSON Level Editor", false, 11)]
    private static void OpenJsonLevelEditor()
    {
        Initialize();
    }

    private static JsonLevelEditor window;
    int width = 150;
    int widthSmall = 40;
    int height = 16;

    int spaceX = 80;
    int spaceY = 18;

    float contentWidth = 0.0f;
    float contentHeight = 0.0f;

    Vector2 scrollPos;


    static string[] colorOptions;
    static string[] moveTypeOptions;
    static string[] panelTypeOptions;

    static PanelColor color = PanelColor.Red;
    static MoveType moveType = MoveType.Square;
    static PanelType panelType = PanelType.Normal;

    public static void Initialize()
    {
        window = (JsonLevelEditor)GetWindow(typeof(JsonLevelEditor));
        window.minSize = new Vector2(500, 300);
        //window.Show();

        colorOptions = CUtility.GetPopupOption(color);
        moveTypeOptions = CUtility.GetPopupOption(moveType);
        panelTypeOptions = CUtility.GetPopupOption(panelType);
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
        Rect contentRect = new Rect(startX, startY, contentWidth - 20, contentHeight);

        GUI.color = new Color(0.85f, 0.85f, 0.85f, 1.0f);
        GUI.Box(visibleRect, "");

        GUI.color = Color.white;

        scrollPos = GUI.BeginScrollView(visibleRect, scrollPos, contentRect);

        startY = DrawLevelList(startX, startY + 5);
        contentWidth = (subWaveBoxWidth + 10) * maxSubWaveSize + 20;

        contentHeight = startY - visibleRect.y - spaceY * 0.5f;

        GUI.EndScrollView();
    }

    float DrawGeneralSetting(float startX, float startY)
    {
        EditorGUI.LabelField(new Rect(startX, startY, width, height), new GUIContent("Levels List : "));
        EditorGUI.IntField(new Rect(startX + spaceX, startY, widthSmall, height), CGameManager.Instance.LevelDataList.Count);

        if (GUI.Button(new Rect(startX + spaceX + 40, startY, widthSmall, height), "-1"))
        {
            if (CGameManager.Instance.LevelDataList.Count > 1)
                CGameManager.Instance.LevelDataList.RemoveAt(CGameManager.Instance.LevelDataList.Count - 1);
        }
        if (GUI.Button(new Rect(startX + spaceX + 80, startY, widthSmall, height), "+1"))
        {
            CGameManager.Instance.LevelDataList.Add(new LevelData());
        }

        if (GUI.Button(new Rect(window.position.width - 130, startY, width - 40, height + 15), "Save"))
            CJsonFileManager.Instance.SaveLevelData();

        if (GUI.Button(new Rect(window.position.width - 260, startY, width - 40, height + 15), "Load"))
            CJsonFileManager.Instance.LoadLevelData();

        return startY + spaceY;
    }



    public List<bool> levelFoldList = new List<bool>();
    public int removeIndex = -1;

    float DrawLevelList(float startX, float startY)
    {
        for (int i = 0; i < CGameManager.Instance.LevelDataList.Count; i++)
        {
            if (levelFoldList.Count <= i) levelFoldList.Add(false);

            levelFoldList[i] = EditorGUI.Foldout(new Rect(startX, startY, widthSmall * 2, 15), levelFoldList[i], "Level - " + (i + 1).ToString());

            if (removeIndex != i)
            {
                if (GUI.Button(new Rect(startX + widthSmall * 2 + 10, startY, widthSmall * 1.5f, 15), "Remove"))
                    removeIndex = i;
                if (GUI.Button(new Rect(startX + widthSmall * 3.5f + 12, startY, widthSmall * 1.5f, 15), "Insert"))
                    CGameManager.Instance.LevelDataList.Insert(i, new LevelData());
            }
            else
            {
                if (GUI.Button(new Rect(startX + widthSmall * 2 + 10, startY, widthSmall * 1.5f, 15), "Cancel"))
                    removeIndex = -1;

                GUI.color = new Color(1.0f, 0.2f, 0.2f, 1.0f);
                if (GUI.Button(new Rect(startX + widthSmall * 3.5f + 12, startY, widthSmall * 1.5f, 15), "Confirm"))
                {
                    CGameManager.Instance.LevelDataList.RemoveAt(i);
                    removeIndex = -1;
                    i -= 1;
                    continue;
                }
                GUI.color = Color.white;
            }

            LevelData levelData = CGameManager.Instance.LevelDataList[i];

            if (levelFoldList[i])
            {
                startX += 15;

                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), new GUIContent("Waves List : "));

                EditorGUI.IntField(new Rect(startX + spaceX, startY, widthSmall, height), levelData.waveList.Count);

                if (GUI.Button(new Rect(startX + spaceX + 40, startY, widthSmall, height), "-1"))
                {
                    if (levelData.waveList.Count > 1)
                        levelData.waveList.RemoveAt(levelData.waveList.Count - 1);
                }
                if (GUI.Button(new Rect(startX + spaceX + 80, startY, widthSmall, height), "+1"))
                {
                    levelData.waveList.Add(new TTPP.PanelWave());
                }

                float cachedY = startY + spaceY;

                startY = DrawWaveList(startX, startY, levelData);

                startX -= 15;
            }
            else
            {
                float cachedX = startX;
                startX += 180 + (removeIndex == i ? widthSmall * 1.5f : 0);

                startX = cachedX;

                startY += spaceY;
            }

        }

        return startY + spaceY * 2;
    }


    public int maxSubWaveSize = 1;
    public List<bool> waveFoldList = new List<bool>();

    float DrawWaveList(float startX, float startY, LevelData data)
    {
        maxSubWaveSize = 1;
        for (int i = 0; i < data.waveList.Count; i++)
        {
            if (waveFoldList.Count <= i) waveFoldList.Add(false);

            startY += spaceY;

            waveFoldList[i] = EditorGUI.Foldout(new Rect(startX, startY, widthSmall * 2, 15), waveFoldList[i], "Wave - " + (i + 1).ToString());

            EditorGUI.LabelField(new Rect(startX + 250, startY, width, height), "Gen Time: ");
            data.waveList[i].genTime = EditorGUI.FloatField(new Rect(startX + spaceX + 250, startY, widthSmall, height),
                data.waveList[i].genTime);

            if (removeIndex != i)
            {
                if (GUI.Button(new Rect(startX + widthSmall * 2 + 10, startY, widthSmall * 1.5f, 15), "Remove"))
                    removeIndex = i;
                if (GUI.Button(new Rect(startX + widthSmall * 3.5f + 12, startY, widthSmall * 1.5f, 15), "Insert"))
                    data.waveList.Insert(i, new TTPP.PanelWave());
            }
            else
            {
                if (GUI.Button(new Rect(startX + widthSmall * 2 + 10, startY, widthSmall * 1.5f, 15), "Cancel"))
                    removeIndex = -1;

                GUI.color = new Color(1.0f, 0.2f, 0.2f, 1.0f);
                if (GUI.Button(new Rect(startX + widthSmall * 3.5f + 12, startY, widthSmall * 1.5f, 15), "Confirm"))
                {
                    data.waveList.RemoveAt(i);
                    removeIndex = -1;
                    i -= 1;
                    continue;
                }
                GUI.color = Color.white;
            }

            TTPP.PanelWave wave = data.waveList[i];

            if (waveFoldList[i])
            {
                startX += 15;

                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), new GUIContent("SubWave "));

                EditorGUI.IntField(new Rect(startX + spaceX, startY, widthSmall, height), wave.subWaveList.Count);

                if (GUI.Button(new Rect(startX + spaceX + 40, startY, widthSmall, height), "-1"))
                {
                    if (wave.subWaveList.Count > 1)
                        wave.subWaveList.RemoveAt(wave.subWaveList.Count - 1);
                }
                if (GUI.Button(new Rect(startX + spaceX + 80, startY, widthSmall, height), "+1"))
                {
                    wave.subWaveList.Add(new TTPP.PanelSubWave());
                }

                float cachedY = startY + spaceY;

                for (int j = 0; j < wave.subWaveList.Count; j++)
                {
                    startY = DrawSubWaveBox(startX + (j * subWaveBoxWidth + 10), cachedY, wave.subWaveList[j]);
                }

                //startY += spaceY * 2.0f;

                startX -= 15;
            }

            maxSubWaveSize = Mathf.Max(waveFoldList[i] ? data.waveList[i].subWaveList.Count : 1, maxSubWaveSize);
        }

        return startY + spaceY;
    }


    private float subWaveBoxWidth = 0;
    private float subWaveBoxHeight = 0;
    float DrawSubWaveBox(float startX, float startY, TTPP.PanelSubWave subWave)
    {
        float spaceX = 60;
        float cachedY = startY;
        width -= 10;

        subWaveBoxWidth = spaceX + width + 5;
        GUI.Box(new Rect(startX, startY, subWaveBoxWidth, subWaveBoxHeight), "");

        startX += 5;
        startY += 3;


        int panelColor = (int)subWave.info.panelColor;
        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "Color: ");
        panelColor = EditorGUI.Popup(new Rect(startX + spaceX, startY, widthSmall * 2, height), panelColor, colorOptions);
        subWave.info.panelColor = (PanelColor)panelColor;

        int moveType = (int)subWave.info.moveType;
        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "MoveType: ");
        moveType = EditorGUI.Popup(new Rect(startX + spaceX, startY, widthSmall * 2, height), moveType, moveTypeOptions);
        subWave.info.moveType = (MoveType)moveType;

        int panelType = (int)subWave.info.panelType;
        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "PanelType: ");
        panelType = EditorGUI.Popup(new Rect(startX + spaceX, startY, widthSmall * 2, height), panelType, panelTypeOptions);
        subWave.info.panelType = (PanelType)panelType;

        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "Speed: ");
        subWave.info.moveSpeed = EditorGUI.FloatField(new Rect(startX + spaceX, startY, widthSmall, height),
            subWave.info.moveSpeed);

        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "Score: ");
        subWave.info.score = EditorGUI.FloatField(new Rect(startX + spaceX, startY, widthSmall, height),
            subWave.info.score);

        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "Scale: ");
        subWave.info.scoreScale = EditorGUI.FloatField(new Rect(startX + spaceX, startY, widthSmall, height),
            subWave.info.scoreScale);

        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "Volume: ");
        subWave.info.panelVolume = EditorGUI.FloatField(new Rect(startX + spaceX, startY, widthSmall, height),
            subWave.info.panelVolume);


        float x = 0.0f; float y = 0.0f;
        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "GenPos: ");
        EditorGUI.LabelField(new Rect(startX + 55, startY, width, height), "X ");
        x = EditorGUI.FloatField(new Rect(startX + spaceX + 5, startY, widthSmall, height), subWave.info.generatePos.x);
        EditorGUI.LabelField(new Rect(startX + spaceX + 55, startY, width, height), "Y ");
        y = EditorGUI.FloatField(new Rect(startX + spaceX * 2 + 5, startY, widthSmall, height), subWave.info.generatePos.y);
        subWave.info.generatePos = new Vector2(x, y);

        startY += 10;

        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "Delay: ");
        subWave.delay = EditorGUI.FloatField(new Rect(startX + spaceX, startY, widthSmall, height), subWave.delay);
        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "Count: ");
        subWave.spawnMax = EditorGUI.IntField(new Rect(startX + spaceX, startY, widthSmall, height), subWave.spawnMax);
        //EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "CountVal: ");
        //subWave.spawnCount = EditorGUI.IntField(new Rect(startX + spaceX, startY, widthSmall, height), subWave.spawnMax);
        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "Interval: ");
        subWave.interval = EditorGUI.FloatField(new Rect(startX + spaceX, startY, widthSmall, height), subWave.interval);


        width += 10;
        subWaveBoxHeight = startY - cachedY + spaceY + 2;


        return startY + spaceY;
    }

}
