using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TTPP;

public static class CUtility
{
    public static int[] StringToIntArray(string str, params char[] separator)
    {
        List<int> intList = new List<int>();
        string[] strs = str.Split(separator);

        foreach (string s in strs)
        {
            int num;

            if (int.TryParse(s.Trim(), out num))
            {
                intList.Add(num);
            }
        }

        return intList.ToArray();
    }

    public static double[] StringToDoubleArray(string str, params char[] separator)
    {
        List<double> doubleList = new List<double>();
        string[] strs = str.Split(separator);

        foreach (string s in strs)
        {
            double num;

            if (double.TryParse(s.Trim(), out num))
            {
                doubleList.Add(num);
            }
        }

        return doubleList.ToArray();
    }


    public static string[] GetPopupOption<T>(T enumName)
    {
        int len = Enum.GetNames(typeof(T)).Length;

        string[] option = new string[len];

        option = Enum.GetNames(typeof(T));

        return option;
    }

    public static Transform GetTopParent(Collider col, string tag)
    {
        return GetTopParent(col.gameObject, tag);
    }

    public static Transform GetTopParent(GameObject obj, string tag)
    {
        Transform parent = obj.transform.parent;

        if (parent != null)
        {
            if (parent.tag.Equals(tag) == false)
            {
                while (parent.parent != null)
                {
                    parent = parent.parent;

                    if (parent.tag.Equals(tag))
                    {
                        break;
                    }
                }
            }
        }

        return parent;
    }

    public static float DecreaseHealthPoint(UnitInfo info, float val)
    {
        float resultDamage = 0.0f;

        // 残りのシールドポイントがダメージより大きい場合
        if (info.shieldPoint > val)
        {
            info.shieldPoint -= val;
            resultDamage = 0;
        }
        else if (info.shieldPoint <= val)
        {
            float setOffDmg = info.shieldPoint;
            info.shieldPoint = 0;

            resultDamage = val - setOffDmg;
            info.healthPoint -= resultDamage;
        }

        if (info.healthPoint < 0)
            info.healthPoint = 0;

        return resultDamage;
    }

    public static int GetEnemyCount()
    {
        return UnityEngine.Object.FindObjectsOfType<CEnemy>().Length;
    }

    public static IEnumerator ShowEffect(EffectType type, Vector3 pos)
    {
        GameObject obj = CEffectManager.Instance.ShowEffect(type, pos);

        yield return new WaitForSeconds(1.5f);

        obj.SetActive(false);
    }
}
