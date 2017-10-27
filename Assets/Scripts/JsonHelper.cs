using System;
using UnityEngine;
using System.Collections.Generic;

public class JsonHelper
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] array = new T[] { };
        public List<T> list = new List<T> { };
    }

    public static T[] JsonToArray<T>(string json)
    {
        string newJson = json.Contains("array") ? json : "{ \"array\": " + json + "}";

        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);

        return wrapper.array;
    }

    public static string ArrayToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.array = array;

        return JsonUtility.ToJson(wrapper);
    }
}
