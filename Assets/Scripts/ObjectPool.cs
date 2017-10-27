using System;
using UnityEngine;

/// <summary>
/// 毎回新しくInstansiate()とDestroy()を乱用すると負担がかかるのでプールを導入。
/// </summary>
public class ObjectPool : MonoBehaviour
{
    private GameObject[] pool;
    private GameObject prefab;

    private const int BASIC_POOL_SIZE = 100;

    /// <summary>
    /// プールを初期化。
    /// </summary>
    /// <param name="_objectToPool"></param>
    /// <param name="poolSize"></param>
    public void InitPool(GameObject _objectToPool, int poolSize = BASIC_POOL_SIZE)
    {
        pool = new GameObject[poolSize];
        prefab = _objectToPool;

        for(int i = 0; i < pool.Length; i++)
        {
            CreateObject(i, _objectToPool);
        }
    }

    /// <summary>
    /// オブジェクトを生成してオフにする。
    /// </summary>
    /// <param name="indexOfObject"></param>
    /// <param name="objectToPool"></param>
    private void CreateObject(int indexOfObject, GameObject objectToPool)
    {
        pool[indexOfObject] = Instantiate(objectToPool) as GameObject;
        pool[indexOfObject].SetActive(false);
    }

    /// <summary>
    /// プールにあるオブジェクトを取得。
    /// </summary>
    /// <returns></returns>
    public GameObject GetObject()
    {
        for(int i = 0; i < pool.Length; i++)
        {
            if(pool[i] != null)
            {
                if(!pool[i].activeSelf)
                {
                    //pool[i].SetActive(true);
                    return pool[i];
                }
            }

            else
            {
                CreateObject(i, prefab);
            }
        }

        Debug.LogError("Method GetObject() returns null.");

        return null;
    }

    public int GetIndex(int i)
    {
        int index;

        index = Array.IndexOf(pool, pool[i]);

        return index;
    }
}
