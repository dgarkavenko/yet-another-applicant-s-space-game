using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleCache<T> where T : Component
{

    public void Log() {
        Debug.Log(_prefab.name);
    }



    private T _prefab;
    private Stack<T> _stack = new Stack<T>();

    public SimpleCache(T go, int preCached)
    {

        if(go == null)
        {
            Debug.LogError(typeof(T).ToString() + " cache not inited");
            return;
        }

        _prefab = go;

        for (int i = 0; i < preCached; i++)
        {
            _stack.Push(NewInstance());
        }
    }

    private T NewInstance()
    {
        var i = (T)GameObject.Instantiate(_prefab);
        i.gameObject.SetActive(false);
        return i;
    }

    public void Push(T instance)
    {
        instance.gameObject.SetActive(false);
        _stack.Push(instance);
    }


    public T Pop()
    {
        var i = _stack.Count > 0 ? _stack.Pop() : NewInstance();
        i.gameObject.SetActive(true);
        return i;
    }


}