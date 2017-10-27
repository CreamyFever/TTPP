using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SingletonPattern;

using TTPP;

public class CLevelManager : CSingletonPattern<CLevelManager>
{    
    public GameObject unlockButton;
    public GameObject lockButton;

    public LevelData[] levelData;

    public LevelStatus[] levelStatuses;

    public LevelData[] LevelData
    {
        get { return levelData; }
        set { levelData = value; }
    }

    public LevelStatus[] LevelStatuses
    {
        get { return levelStatuses; }
        set { levelStatuses = value; }
    }

    public int CurrentPage { get; private set; }

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        GetData();
        InitLevelStatus();
        CreateLevelButtons();
    }

    void GetData()
    {
        levelData = CDataManager.GetData<LevelData[]>(Constants.KEY_LEVEL_DATA);
    }

    void InitLevelStatus()
    {
        if (levelData == null)
            return;

        int len = LevelData.Length;

        LevelStatus[] statuses = new LevelStatus[len];

        for(int i = 0; i < len; i++)
        {
            LevelStatus status = new LevelStatus();
            status.index = i;
            status.clearCount = Constants.LEVEL_STATUS_LOCKED;
            statuses[i] = status;
        }

        statuses[0].clearCount = Constants.LEVEL_STATUS_UNLOCKED;
        statuses[1].clearCount = Constants.LEVEL_STATUS_UNLOCKED;

        this.levelStatuses = statuses;

        string toJson = JsonHelper.ArrayToJson(statuses);
        PlayerPrefs.SetString(Constants.KEY_LEVEL_STATUS, toJson);  // リリースするなら暗号化が必要
    }

    void CreateLevelButtons()
    {
        if (levelStatuses == null)
            return;
        
        for(int i = 0; i < levelStatuses.Length; i++)
        {
            LevelStatus status = levelStatuses[i];

            GameObject prefab = (status.clearCount > Constants.LEVEL_STATUS_LOCKED) ? unlockButton : lockButton;
            GameObject button = Instantiate(prefab);

            Transform panelTrans = GameObject.Find("LevelPanel").transform;

            Text btnText = button.GetComponentInChildren<Text>();

            if(btnText != null)
            {
                btnText.text = (status.index + 1).ToString();
            }

            button.transform.SetParent(panelTrans);
        }

    }

    public void OnLevelButtonClick(Text btnText)
    {
        int level = int.Parse(btnText.text) - 1;

        Debug.Log("Clicked Level = " + (level+1));

        PlayerPrefs.SetInt(Constants.KEY_PLAY_LEVEL, level);

        CSoundManager.Instance.ChangeBgm(Constants.SOUND_BGM_GAME_FIELD);
        SceneManager.LoadScene("Game");
    }
}
