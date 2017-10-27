using UnityEngine;
using System.IO;
using SingletonPattern;

using TTPP;

public class CJsonFileManager : CSingletonPattern<CJsonFileManager>
{
    private void Awake()
    {
        instance = this;
    }

    public void SaveFile(string content, string fileName)
    {
        string path = Path.Combine(Application.dataPath + "/Resources/Data/", fileName + ".json");

        if(File.Exists(path))
        {
            File.Delete(path);
        }

        File.WriteAllText(path, content);
    }

    /// <summary>
    /// Androidでファイルをロードするために作成した。
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string LoadFile(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);

        if(textAsset != null)
        {
            return textAsset.text;
        }
        Debug.LogError("File Open Failed!");
        return "";
    }

    public void SaveLevelData()
    {
        string toJson = JsonHelper.ArrayToJson(CGameManager.Instance.LevelDataList.ToArray());
        File.WriteAllText(Application.dataPath + "/Resources/Data/LevelData.json", toJson);
    }

    public void LoadLevelData()
    {
#if UNITY_EDITOR
        string jsonString = File.ReadAllText(Application.dataPath + "/Resources/Data/LevelData.json");
#elif UNITY_ANDROID
        string jsonString = LoadFile(Constants.LEVEL_DATA_RES_PATH);
#endif
        var data = JsonHelper.JsonToArray<LevelData>(jsonString);

        CGameManager.Instance.LevelDataList.Clear();

        for (int i = 0; i < data.Length; i++)
        {
            CGameManager.Instance.LevelDataList.Add(data[i]);
        }
    }

}
