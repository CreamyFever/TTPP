using System.Collections.Generic;
using SingletonPattern;

public class CDataManager : CSingletonPattern<CDataManager>
{
    private static Dictionary<string, object> dict = new Dictionary<string, object>();

    public static void AddData<T>(string key, T value) where T : class
    {
        if(dict.ContainsKey(key))
        {
            dict.Remove(key);
        }

        dict.Add(key, value);
    }

    public static T GetData<T>(string key) where T : class
    {
        object value;

        return dict.TryGetValue(key, out value) ? value as T : null;
    }
}
