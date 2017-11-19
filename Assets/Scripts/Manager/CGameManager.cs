using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonPattern;

using TTPP;

public class CGameManager : CSingletonPattern<CGameManager>
{
    public bool hasFocus = false;
    public bool isPause = false;
    public bool isGameOver = false;
    public bool isClear = false;


    #region Game
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

    LevelStatus[] statuses;

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
    #endregion

    #region Panel
    public Sprite[] panelSprites;

    [SerializeField]
    GameObject m_panel;
    ObjectPool m_panelPool;

    [SerializeField]
    int m_panelCount;
    public int PanelCount
    {
        get { return m_panelCount; }
        set { m_panelCount = value; }
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
    #endregion

    #region Enemy
    [SerializeField]
    GameObject m_enemy;
    ObjectPool m_enemyPool;

    List<EnemyInfo> enemyList = new List<EnemyInfo>();
    public List<EnemyInfo> EnemyList
    {
        get { return enemyList; }
        set { enemyList = value; }
    }

    #endregion

    #region Spawn
    int leftPanelCount;     // UIに示す残りのパネル数

    int allPanelCount;          // 一つのウェイブで現れるパネルの総数
    public int AllPanelCount
    {
        get { return allPanelCount; }
        set { allPanelCount = value; }
    }
    #endregion

    private void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(instance);

        Init();
        GetData();

        InitPanels();
        InitEnemies();
    }

    IEnumerator Start()
    {
        InitializeToSpawnPanels();
        InitializeToSpawnEnemy();

        CSoundManager.Instance.ChangeBgm(Constants.SOUND_BGM_GAME_FIELD);

        while (true)
        {
            yield return null;

            CUIManager.Instance.ShowTopUI(leftPanelCount);

            if (currentWave == levelData.waveList.Count - 1)
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

        string json = PlayerPrefs.GetString(Constants.KEY_LEVEL_STATUS);
        statuses = JsonHelper.JsonToArray<LevelStatus>(json);
    }

    void InitPanels()
    {
        panelSprites = Resources.LoadAll<Sprite>("Images/TapPanels");

        m_panelPool = gameObject.AddComponent<ObjectPool>();
        m_panelPool.InitPool(m_panel, m_panelCount);
    }

    void InitEnemies()
    {
        m_enemyPool = gameObject.AddComponent<ObjectPool>();
        m_enemyPool.InitPool(m_enemy, 4);
    }

    void GetData()
    {
        Debug.Log("LEVEL = " + (playLevel+1).ToString() + ", WAVE = " + (currentWave+1).ToString());
        var data = CGlobalManager.GetData<LevelData[]>(Constants.KEY_LEVEL_DATA);

        levelDataList.Clear();

        for (int i = 0; i < data.Length; i++)
        {
            levelDataList.Add(data[i]);
        }
    }

    void SaveLevelStatus(bool isCleared)
    {
        if(isCleared)
        {
            statuses[playLevel].clearCount++;
        }

        string json = JsonHelper.ArrayToJson(statuses);
        PlayerPrefs.SetString(Constants.KEY_LEVEL_STATUS, json);    // リリースするなら暗号化が必要
    }

    /// <summary>
    /// 次のレベルを開放します。
    /// </summary>
    public void OpenNextStage()
    {
        if(playLevel < statuses.Length - 1)
        {
            LevelStatus next = statuses[playLevel + 1];

            if(next.clearCount == Constants.LEVEL_STATUS_LOCKED)
            {
                next.clearCount = Constants.LEVEL_STATUS_UNLOCKED;
            }
        }

        SaveLevelStatus(true);
    }

    #region PanelMethod

    public void GeneratePanels(PanelSubWave subwave)
    {
        GameObject panel;

        panel = m_panelPool.GetObject();

        if (panel == null)
            return;

        panel.transform.position = subwave.info.generatePos;

        SetPanelInfo(panel, subwave);
        panel.SetActive(true);
    }

    public void SetPanelInfo(GameObject panel, PanelSubWave subwave)
    {
        PanelInfo info = panel.GetComponent<CPanel>().panelInfo;
        info.subWaveIndex = subwave.index;
        info.panelColor = subwave.info.panelColor;
        info.moveType = subwave.info.moveType;
        info.panelType = subwave.info.panelType;
        info.moveSpeed = subwave.info.moveSpeed;
        info.score = subwave.info.score;
        info.scoreScale = subwave.info.scoreScale;
        info.panelVolume = subwave.info.panelVolume;
        info.generatePos = subwave.info.generatePos;
    }
    #endregion

    #region SpawnMethod
    public void InitializeToSpawnPanels()
    {
        Coroutine[] coroutine;

        PanelWave wave = levelDataList[playLevel].waveList[currentWave];

        coroutine = new Coroutine[wave.subWaveList.Count];

        for (int i = 0; i < wave.subWaveList.Count; i++)
        {
            wave.subWaveList[i].index = i;

            wave.subWaveList[i].spawnCount = wave.subWaveList[i].spawnMax;
            leftPanelCount += wave.subWaveList[i].spawnCount;

            if (coroutine[i] != null)
                StopCoroutine(coroutine[i]);

            coroutine[i] = StartCoroutine(SpawnPanels(wave, i));
        }
        allPanelCount = leftPanelCount;
    }


    public void InitializeToSpawnEnemy()
    {
        PanelWave wave = levelDataList[playLevel].waveList[currentWave];

        for (int i = 0; i < wave.enemyList.Count; i++)
        {
            AppearEnemies(wave, i);
        }
    }

    void AppearEnemies(PanelWave wave, int enemyIndex)
    {
        EnemyInfo info = wave.enemyList[enemyIndex];
        wave.enemyList[enemyIndex].index = enemyIndex;

        GenerateEnemies(info, wave.enemyList.Count);
    }


    IEnumerator SpawnPanels(PanelWave wave, int subWaveIndex)
    {
        yield return new WaitForSeconds(2.0f);

        PanelSubWave subwave = wave.subWaveList[subWaveIndex];

        WaitForSeconds waitSec = new WaitForSeconds(subwave.delay);
        yield return waitSec;

        waitSec = new WaitForSeconds(subwave.interval);

        while (CUtility.GetEnemyCount() != 0 && leftPanelCount > 0)
        {
            if (subwave.spawnCount > 0)
            {
                GeneratePanels(subwave);
                subwave.spawnCount -= 1;
                leftPanelCount -= 1;

                yield return waitSec;
            }
            else if (subwave.spawnCount == 0)
            {
                yield break;
            }

            yield return null;
        }
    }

    public void ClearAllPanels()
    {
        CPanel[] panels = FindObjectsOfType<CPanel>();

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].gameObject.SetActive(false);
        }

        leftPanelCount = 0;
    }
    #endregion

    #region EnemyMethod
    public void GenerateEnemies(EnemyInfo info, int count)
    {
        GameObject enemy;

        enemy = m_enemyPool.GetObject();

        if (enemy == null)
            return;

        enemy.transform.position = AssignPosition(info, count);

        SetEnemyInfo(enemy, info);
        enemy.SetActive(true);
    }

    /// <summary>
    /// 登場する敵の数で座標を設定する。
    /// </summary>
    /// <param name="info"></param>
    /// <param name="count"></param>
    Vector3 AssignPosition(EnemyInfo info, int count)
    {
        float startX;
        switch (count)
        {
            case 1:
                startX = 0;
                break;
            case 2:
                startX = -1;
                break;
            case 3:
                startX = -2;
                break;
            case 4:
                startX = -3;
                break;
            default:
                startX = 0;
                break;
        }

        return new Vector3(startX + info.index * 2, 8, 1);
    }

    void SetEnemyInfo(GameObject enemy, EnemyInfo _info)
    {
        EnemyInfo info = enemy.GetComponent<CEnemy>().enemyInfo;
        info.index = _info.index;
        info.sprite = _info.sprite;
        info.name = _info.name;
        info.healthPoint = _info.healthPoint;
        info.attackPoint = _info.attackPoint;
        info.shieldPoint = _info.shieldPoint;
        info.leftTurn = _info.leftTurn;

        info.maxHealthPoint = info.healthPoint;
        info.maxAttackPoint = info.attackPoint;
        info.maxShieldPoint = info.shieldPoint;
    }
    #endregion
}
