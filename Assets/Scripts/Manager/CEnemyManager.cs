using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonPattern;

using TTPP;

public class CEnemyManager : CSingletonPattern<CEnemyManager>
{
    List<EnemyInfo> enemyList = new List<EnemyInfo>();
    public List<EnemyInfo> EnemyList
    {
        get { return enemyList; }
        set { enemyList = value; }
    }

    [SerializeField]
    GameObject m_enemy;
    ObjectPool m_enemyPool;



    private void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(instance);

        m_enemyPool = gameObject.AddComponent<ObjectPool>();
        m_enemyPool.InitPool(m_enemy, 4);
    }
    

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
        switch(count)
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
        info.prefab = _info.prefab;
        info.name = _info.name;
        info.healthPoint = _info.healthPoint;
        info.attackPoint = _info.attackPoint;
        info.shieldPoint = _info.shieldPoint;
        info.leftTurn = _info.leftTurn;

        info.maxHealthPoint = info.healthPoint;
        info.maxAttackPoint = info.attackPoint;
        info.maxShieldPoint = info.shieldPoint;
    }

}
