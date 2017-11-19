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

    float healthPoint = 100.0f;
    float shieldPoint = 20.0f;
    float attackPoint = 10.0f;
    
    Stack<PanelColor> touchedPanelColorStack = new Stack<PanelColor>();
    

    private void Awake()
    {
        info = new PlayerInfo("PLAYER", healthPoint, attackPoint, shieldPoint);
        InitializePlayerValue();

        wavePanelCount = 0;
    }
    
    void Update ()
    {
        RecoverShieldPoint();

        CUIManager.Instance.ShowComboUI(combo, needToCombo);
        CUIManager.Instance.ShowBottomUI(info, touchedPanelCount);

        if (wavePanelCount == CGameManager.Instance.AllPanelCount)
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

    /// <summary>
    /// シールドポイントが徐々に回復する。
    /// </summary>
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

    public void ContinueGame()
    {
        isAlive = true;
        info.healthPoint = healthPoint;
        info.shieldPoint = shieldPoint;
        info.attackPoint = attackPoint;

        CGameManager.Instance.ClearAllPanels();
        InitializePlayerValue();
        wavePanelCount = 0;

        CGameManager.Instance.InitializeToSpawnPanels();
    }

    /// <summary>
    /// 敵1体を攻める。
    /// </summary>
    /// <param name="_enemy"></param>
    void AttackEnemy(GameObject _enemy)
    {
        CEnemy enemy = _enemy.GetComponent<CEnemy>();
        float damage = CalculateDamage();
        
        CSoundManager.Instance.PlayEffectSound(Constants.SOUND_ID_PLAYER_ATTACK);
        StartCoroutine(CUtility.ShowEffect(EffectType.Damage, enemy.transform.position));
        enemy.OnDamaged(damage);

        InitializePlayerValue();

        StartCoroutine(GoToNextWaveSingleEnemy(enemy));
    }

    IEnumerator GoToNextWaveSingleEnemy(CEnemy enemy)
    {
        yield return null;
          
        if (CUtility.GetEnemyCount() == 0 && enemy.enemyInfo.healthPoint <= 0.0f)       // 残りの１体を倒したら、次のウェイブへ
        {
            CGameManager.Instance.ClearAllPanels();

            yield return new WaitForSeconds(2.0f);

            wavePanelCount = 0;

            DecideNextWaveState();
        }

        yield break;
    }

    /// <summary>
    /// ボス戦を前にしたり、ステージをクリアしたり
    /// </summary>
    /// <param name="soundID"></param>
    /// <param name="nextWave"></param>
    /// <param name="changeBGM"></param>
    /// <returns></returns>
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
        {
            changeBGM(Constants.SOUND_BGM_RESULT);
            CUIManager.Instance.ShowStageClearPanel(true);
        }
    }

    void DecideNextWaveState()
    {
        if (CGameManager.Instance.CurrentWave == CGameManager.Instance.levelData.waveList.Count - 1) // 最後のウェイブだったら
        {
            StartCoroutine(SpecialEvent(Constants.SOUND_ID_CLEAR, null, CSoundManager.Instance.ChangeBgm));
            CGameManager.Instance.OpenNextStage();
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
        CGameManager.Instance.InitializeToSpawnPanels();
        CGameManager.Instance.InitializeToSpawnEnemy();
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
            StartCoroutine(CUtility.ShowEffect(EffectType.Damage, enemy.transform.position));

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
            CGameManager.Instance.InitializeToSpawnPanels();
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

    /// <summary>
    /// タッチしたパネルを消す(実は非アクティブにする)。
    /// </summary>
    /// <param name="_panel"></param>
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
    /// 違う色をタッチしたらコンボスタックを増やしなおすようにする。
    /// </summary>
    /// <param name="_panel"></param>
    void CheckCombo(PanelColor color)
    {
        if (needToCombo > 0)
        {
            if (color == touchedPanelColorStack.Peek())         // タッチしたパネルと以前のパネルの色が同じならコンボスタック増加
            {
                needToCombo += 1;

                if (needToCombo == NEED_TO_COMBO)               // 同じ色のパネルを連続に三つタッチしたらコンボを増やす。
                {
                    needToCombo = 0;                            // コンボスタック初期化
                    combo++;

                    CSoundManager.Instance.PlayEffectSound(Constants.SOUND_ID_GOT_COMBO);
                }
            }

            else needToCombo = 1;

            touchedPanelColorStack.Clear();
        }

        else needToCombo = 1;
    }


    /// <summary>
    /// ダメージを受けたら呼び出される。
    /// </summary>
    /// <param name="damage"></param>
    public void OnDamaged(float damage)
    {
        CUtility.DecreaseHealthPoint(info, damage);

        combo = 0; needToCombo = 0;                 // コンボが途切れた。
        touchedPanelColorStack.Clear();

        if (info.healthPoint <= 0.0f)
        {
            isAlive = false;
            CGameManager.Instance.ClearAllPanels();
            CUIManager.Instance.ShowGameOverPanel(true);    // ゲームオーバーパネルを有効にする。
        }
    }
}
