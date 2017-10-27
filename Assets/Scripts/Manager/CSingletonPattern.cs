using UnityEngine;

namespace SingletonPattern
{
    public class CSingletonPattern<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<T>();
                    obj.name = typeof(T).ToString();
                    //DontDestroyOnLoad(obj);
                }

                return instance;
            }
        }
    }
}