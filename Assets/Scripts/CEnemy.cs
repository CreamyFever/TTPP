using System.Collections;
using UnityEngine;

using TTPP;

public class CEnemy : MonoBehaviour
{
    public EnemyInfo enemyInfo;
    private PanelWave wave;
    
    public SpriteRenderer ren;

    CPlayer player;
    public int leftTurnToInit;

    public bool isAlive = true;

    /// <summary>
    /// オブジェクトプールにあるオブジェクトの使い回しなので、アクティブ化の際に呼び出すようにする。
    /// </summary>
    private void OnEnable()
    {
        isAlive = true;
        InitializeEnemies();
        player = GameObject.Find("Player").GetComponent<CPlayer>();

        ren.sprite = enemyInfo.sprite;
        leftTurnToInit = enemyInfo.leftTurn;
    }

    private void Update()
    {
        RecoverShieldPoint();
        CUIManager.Instance.ShowEnemyUI(enemyInfo, transform.position);
    }


    void InitializeEnemies()
    {
        int level = CGameManager.Instance.PlayLevel;
        int currentWave = CGameManager.Instance.CurrentWave;
        wave = CGameManager.Instance.LevelDataList[level].waveList[currentWave];

        ren = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void RecoverShieldPoint()
    {
        if (enemyInfo.shieldPoint < enemyInfo.maxShieldPoint)
        {
            enemyInfo.shieldPoint += Time.deltaTime * 0.5f;
        }
    }

    

    public void OnDamaged(float damage)
    {
        CUtility.DecreaseHealthPoint(enemyInfo, damage);

        if (enemyInfo.healthPoint <= 0.0f)
        {
            isAlive = false;
            CSoundManager.Instance.PlayEffectSound(Constants.SOUND_ID_ENEMY_DESTROYED);
            StartCoroutine(WaitForBeingInactive());
        }
    }

    /// <summary>
    /// 敵を直ぐに非アクティブ化させると敵のUIが消えない現象が起こる。
    /// 次のフレームで非アクティブ化させる。
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitForBeingInactive()
    {
        yield return null;

        gameObject.SetActive(false);
        yield break;
    }

    public void AttackPlayer()
    {
        float damage = enemyInfo.attackPoint;

        if(enemyInfo.leftTurn == 0)
        {
            player.OnDamaged(damage);
            CSoundManager.Instance.PlayEffectSound(Constants.SOUND_ID_ENEMY_ATTACK);

            enemyInfo.leftTurn = leftTurnToInit;
        }
    }
}
