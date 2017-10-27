using TTPP;
using SingletonPattern;
using UnityEngine.SceneManagement;

public class CTitleManager : CSingletonPattern<CTitleManager>
{
    private void Awake()
    {
        instance = this;
    }
    

    public void OnPlayButtonClicked()
    {
        LoadData();

        CSoundManager.Instance.ChangeBgm(Constants.SOUND_BGM_SELECT_LEVEL);
        SceneManager.LoadScene("SelectLevel");
    }

    void LoadData()
    {
        string levelData = CJsonFileManager.Instance.LoadFile(Constants.LEVEL_DATA_RES_PATH);
        LevelData[] levelDatas = JsonHelper.JsonToArray<LevelData>(levelData);        

        CDataManager.AddData(Constants.KEY_LEVEL_DATA, levelDatas);
    }
}
