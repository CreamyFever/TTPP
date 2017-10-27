using System.Collections;
using UnityEngine;
using SingletonPattern;

using TTPP;

public class CSpawnManager : CSingletonPattern<CSpawnManager>
{
    int leftPanelCount;     // UIに示す残りのパネル数

    int allPanelCount;          // 一つのウェイブで現れるパネルの総数
    public int AllPanelCount
    {
        get { return allPanelCount; }
        set { allPanelCount = value; }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializeToSpawnPanels();
        InitializeToSpawnEnemy();
    }

    private void Update()
    {
        CUIManager.Instance.ShowTopUI(leftPanelCount);
    }

    public void InitializeToSpawnPanels()
    {
        Coroutine[] coroutine;
        int level = CGameManager.Instance.PlayLevel;
        int currentWave = CGameManager.Instance.CurrentWave;

        PanelWave wave = CGameManager.Instance.LevelDataList[level].waveList[currentWave];

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
        //CGameManager.Instance.LevelDataList[level].waveList[currentWave] = wave;
    }
    

    public void InitializeToSpawnEnemy()
    {
        int level = CGameManager.Instance.PlayLevel;
        int currentWave = CGameManager.Instance.CurrentWave;

        PanelWave wave = CGameManager.Instance.LevelDataList[level].waveList[currentWave];

        for (int i = 0; i < wave.enemyList.Count; i++)
        {
            AppearEnemies(wave, i);
        }
    }

    void AppearEnemies(PanelWave wave, int enemyIndex)
    {
        EnemyInfo info = wave.enemyList[enemyIndex];
        wave.enemyList[enemyIndex].index = enemyIndex;

        CEnemyManager.Instance.GenerateEnemies(info, wave.enemyList.Count);
    }
    

    IEnumerator SpawnPanels(PanelWave wave, int subWaveIndex)
    {
        yield return new WaitForSeconds(2.0f);

        PanelSubWave subwave = wave.subWaveList[subWaveIndex];

        WaitForSeconds waitSec = new WaitForSeconds(subwave.delay);
        yield return waitSec;
        
        waitSec = new WaitForSeconds(subwave.interval);

        while (CUtility.GetEnemyCount() != 0　&& leftPanelCount > 0)
        {
            if (subwave.spawnCount > 0)
            {
                CPanelManager.instance.GeneratePanels(subwave);
                subwave.spawnCount -= 1;
                leftPanelCount -= 1;
                
                yield return waitSec;
            }
            else if(subwave.spawnCount == 0)
            {
                yield break;
            }

            yield return null;
        }
    }

    public void ClearAllPanels()
    {
        CPanel[] panels = FindObjectsOfType<CPanel>();

        for(int i = 0; i < panels.Length; i++)
        {
            panels[i].gameObject.SetActive(false);
        }

        leftPanelCount = 0;
    }
}
