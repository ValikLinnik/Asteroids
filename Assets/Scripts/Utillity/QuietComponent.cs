using UnityEngine;
using System.Collections;
using System;

public class QuietComponent<T>: OverrodeOperators where T: Component
{
    private MonoBehaviour _monoBehaviour;
    private T _component;
    public T Component
    {
        get
        {
            if(!_monoBehaviour) throw new NullReferenceException("monoBehaviour is null");
            if(!_component) _component = _monoBehaviour.GetComponent<T>();
            return _component;
        }
    }

    public QuietComponent(MonoBehaviour monoBehaviour)
    {
        _monoBehaviour = monoBehaviour;
    }
}

public class OverrodeOperators
{
    public static implicit operator bool(OverrodeOperators instance)
    {
        return instance != null;
    }
}
