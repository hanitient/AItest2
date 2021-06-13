using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReusableObject : MonoBehaviour,IsReusable
{
    public virtual void OnSpawn()
    {
        gameObject.SetActive(true);
    }
    public virtual void UnSpawn()
    {
        gameObject.SetActive(false);
    }
}
