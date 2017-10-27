using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonPattern;

using TTPP;

public class CGameManager : CSingletonPattern<CGameManager>
{
    public bool isPause = false;
    public bool isGameOver = false;
    public bool isClear = false;

    [SerializeField]
    List<LevelData> levelDataList = new List<LevelData>();
    public List<LevelData> LevelDataList
    {
        get { return levelDataList; }
        set { levelDataList = value; }
    }

    public LevelData levelData
    {
        get { return LevelDataList[playLevel]; }
    }

    List<PanelWave> waveList = new List<PanelWave>();
    public List<PanelWave> WaveList
    {
        get { return waveList; }
        set { waveList = value; }
    }

    public PanelWave PanelWave
    {
        get { return waveList[currentWave]; }
    }

    [SerializeField]
    int playLevel;
    public int PlayLevel
    {
        get { return playLevel; }
        set { playLevel = value; }
    }

    [SerializeField]
    int currentWave;
    public int CurrentWave
    {
        get { return currentWave; }
        set { currentWave = value; }
    }


    private void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(instance);

        Init();
        GetData();
    }

    IEnumerator Start()
    {
        while(true)
        {
            yield return null;

            if(currentWave == levelData.waveList.Count - 1)
            {
                CSoundManager.Instance.ChangeBgm(Constants.SOUND_BGM_BOSS);
                yield break;
            }
        }
    }
    
    void Init()
    {
        isPause = false;
        isGameOver = false;
        isClear = false;

        playLevel = PlayerPrefs.GetInt(Constants.KEY_PLAY_LEVEL);
        currentWave = 0;
    }

    void GetData()
    {
        Debug.Log("LEVEL = " + (playLevel+1).ToString() + ", WAVE = " + (currentWave+1).ToString());
        var data = CDataManager.GetData<LevelData[]>(Constants.KEY_LEVEL_DATA);

        levelDataList.Clear();

        for (int i = 0; i < data.Length; i++)
        {
            levelDataList.Add(data[i]);
        }
    }
}
