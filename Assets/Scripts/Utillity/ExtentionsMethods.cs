using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public static class ExtentionsMethods
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        return collection == null || !collection.Any();
    }

    public static T GetRandomItem<T>(this IEnumerable<T> collection) where T : class 
    {
        return collection.IsNullOrEmpty() ? null : collection.ElementAt(UnityRandom.Range(0, collection.Count()));
    }

    public static void WaitAndDo(this MonoBehaviour mono, float time, Action action)
    {
        if(mono == null) return;
        mono.StartCoroutine(CoroutineWaitAndDo(time, action));
    }

    private static IEnumerator CoroutineWaitAndDo(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        if(action != null) action();
    }
}


