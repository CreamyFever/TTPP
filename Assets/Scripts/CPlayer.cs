using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTPP;

public class CPlayer : MonoBehaviour
{
    bool isAlive = true;
    public PlayerInfo info;

    private int touchedPanelCount;
    private int combo;
    private int needToCombo;

    public int wavePanelCount;      // タッチしたパネルと逃したパネルの和。CSpawnManagerのallPanelCountと比べるために宣言

    const int NEED_TO_COMBO = 3;
    
    Stack<PanelColor> touchedPanelColorStack = new Stack<PanelColor>();
    

    private void Awake()
    {
        info = new PlayerInfo("PLAYER", 100, 10, 20);
        InitializePlayerValue();

        wavePanelCount = 0;
    }
    
    void Update ()
    {
        RecoverShieldPoint();

        CUIManager.Instance.ShowComboUI(combo, needToCombo);
        CUIManager.Instance.ShowBottomUI(info, touchedPanelCount);

        if (wavePanelCount == CSpawnManager.Instance.AllPanelCount)
        {
            AttackAllEnemy();
        }

        if (CInputManager.Instance.Pressed() && isAlive)
        {
            CheckCollider();
        }
	}

    void CheckCollider()
    {
        RaycastHit hit = new RaycastHit();

        Ray ray = Camera.main.ScreenPointToRay(CInputManager.Instance.PressedPos);

        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            if (hit.transform.CompareTag("Panel"))
            {
                DestroyPanels(hit.transform.gameObject);
            }
            else if (hit.transform.CompareTag("Enemy"))
            {
                AttackEnemy(hit.transform.gameObject);
            }
        }
    }   

    public void RecoverShieldPoint()
    {
        if(info.shieldPoint < info.maxShieldPoint)
        {
            info.shieldPoint += Time.deltaTime * 0.5f;
        }
    }

    public void InitializePlayerValue()
    {
        touchedPanelCount = 0;
        combo = 0; needToCombo = 0;
        touchedPanelColorStack.Clear();
    }

    void AttackEnemy(GameObject _enemy)
    {
        CEnemy enemy = _enemy.GetComponent<CEnemy>();
        float damage = CalculateDamage();
        
        CSoundManager.Instance.PlayEffectSound(Constants.SOUND_ID_PLAYER_ATTACK);
        enemy.OnDamaged(damage);

        InitializePlayerValue();

        StartCoroutine(GoToNextWaveSingleEnemy(enemy));
    }

    IEnumerator GoToNextWaveSingleEnemy(CEnemy enemy)
    {
        yield return null;
          
        if (CUtility.GetEnemyCount() == 0 && enemy.enemyInfo.healthPoint <= 0.0f)       // 残りの１体を倒したら、次のウェイブへ
        {
            CSpawnManager.Instance.ClearAllPanels();

            yield return new WaitForSeconds(2.0f);

            wavePanelCount = 0;

            DecideNextWaveState();
        }

        yield break;
    }

    IEnumerator SpecialEvent(int soundID, System.Action nextWave = null, System.Action<int> changeBGM = null)
    {
        CSoundManager.Instance.StopBgm();
        yield return new WaitForSeconds(2.0f);

        if(nextWave != null) CUIManager.Instance.ShowWarningUI(true);

        CSoundManager.Instance.PlayEffectSound(soundID);
        yield return new WaitForSeconds(5.0f);

        CUIManager.Instance.ShowWarningUI(false);

        if (nextWave != null)
            nextWave();
        if (changeBGM != null)
            changeBGM(Constants.SOUND_BGM_RESULT);
    }

    void DecideNextWaveState()
    {
        if (CGameManager.Instance.CurrentWave == CGameManager.Instance.levelData.waveList.Count - 1) // 最後のウェイブだったら
        {
            StartCoroutine(SpecialEvent(Constants.SOUND_ID_CLEAR, null, CSoundManager.Instance.ChangeBgm));
        }
        else if (CGameManager.Instance.CurrentWave == CGameManager.Instance.levelData.waveList.Count - 2)    // ボスウェイブを前にしたら
        {
            StartCoroutine(SpecialEvent(Constants.SOUND_ID_BOSS_APPROACH, NextWave));
        }
        else                                                                                         // ウェイブがまだ残ったら
        {
            NextWave();
        }
    }


    void NextWave()
    {
        CGameManager.Instance.CurrentWave++;
        CSpawnManager.Instance.InitializeToSpawnPanels();
        CSpawnManager.Instance.InitializeToSpawnEnemy();
    }
    

    /// <summary>
    /// 敵1体をターゲットにしないで、最後のパネル1個をタッチしたら呼び出される。集めたダメージは敵の数によって分散される。
    /// </summary>
    public void AttackAllEnemy()
    {
        CEnemy[] enemies = FindObjectsOfType<CEnemy>();
        float damage = CalculateDamage() / enemies.Length * 1.5f;

        foreach (var enemy in enemies)
        {
            enemy.OnDamaged(damage);

            enemy.enemyInfo.leftTurn--;
            enemy.AttackPlayer();
        }
        CSoundManager.Instance.PlayEffectSound(Constants.SOUND_ID_PLAYER_ATTACK);
        wavePanelCount = 0;

        InitializePlayerValue();

        StartCoroutine(GoToNextWave());
    }

    IEnumerator GoToNextWave()
    {
        yield return new WaitForSeconds(2.0f);

        if (CUtility.GetEnemyCount() == 0)       // 敵を全て倒したら、次のウェイブへ
        {
            DecideNextWaveState();
        }
        else
        {                               // 倒しきれなかったら、パネルを生成しなおす。
            CSpawnManager.Instance.InitializeToSpawnPanels();
        }

        yield break;
    }

    /// <summary>
    /// 敵に与えるダメージを計算。
    /// </summary>
    /// <returns>与えるダメージ＝タッチしたパネル数＊(100＋コンボ＊10)％＊攻撃力</returns>
    float CalculateDamage()
    {
        　return touchedPanelCount * (1 + combo * 0.1f) * info.attackPoint;
    }

    void DestroyPanels(GameObject _panel)
    {
        CPanel panel = _panel.GetComponent<CPanel>();
        info.score += panel.panelInfo.score;
        touchedPanelCount += 1;

        PanelColor panelColor = panel.panelInfo.panelColor;
        CUIManager.Instance.GetTouchedPanelColor(panelColor);

        CheckCombo(panelColor);
        touchedPanelColorStack.Push(panelColor);

        CSoundManager.Instance.PlayEffectSound(Constants.SOUND_ID_PANEL_TOUCH);
        panel.gameObject.SetActive(false);

        wavePanelCount++;
    }

    /// <summary>
    /// 前にタッチしたパネルの色と同じなら、コンボが繋げる。
    /// 同じ色のパネルを連続に3回タッチしたらコンボ数が上がる。
    /// 違う色をタッチしたらコンボスタックを貯めなおすようにする。
    /// </summary>
    /// <param name="_panel"></param>
    void CheckCombo(PanelColor color)
    {
        if (needToCombo > 0)
        {
            if (color == touchedPanelColorStack.Peek())
            {
                needToCombo += 1;

                if (needToCombo == NEED_TO_COMBO)
                {
                    needToCombo = 0;
                    combo++;

                    CSoundManager.Instance.PlayEffectSound(Constants.SOUND_ID_GOT_COMBO);
                }
            }

            else needToCombo = 1;

            touchedPanelColorStack.Clear();
        }

        else needToCombo = 1;
    }

    public void OnDamaged(float damage)
    {
        CUtility.DecreaseHealthPoint(info, damage);

        combo = 0; needToCombo = 0;
        touchedPanelColorStack.Clear();

        if (info.healthPoint <= 0.0f)
        {
            isAlive = false;
            Debug.Log("You Died");

            Time.timeScale = 0.0f;
            // 컨티뉴, 나가기 팝업 창 띄우기
        }
    }
}
