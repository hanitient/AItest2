using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance = null;
    public static T Instance
    {
        get { return m_instance;}
        private set { m_instance = value; }
    }
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
            Debug.Log(this.name + "已经创建了singleton实例");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning(name+"单例已经销毁！");
        }
    }
}
