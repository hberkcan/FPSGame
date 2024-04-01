using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class View : MonoBehaviour
{
    public virtual void Open() 
    {
        gameObject.SetActive(true);
    }

    public virtual void Close() 
    {
        gameObject.SetActive(false);
    }
}
