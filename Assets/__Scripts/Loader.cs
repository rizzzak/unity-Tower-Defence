using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T _instance;
    public static T instance
    {
        get {
            if (_instance == null)
                _instance = FindObjectOfType<T>();
            else if (_instance != FindObjectOfType<T>())
                Destroy(FindObjectOfType<T>());
            DontDestroyOnLoad(FindObjectOfType<T>());
            return _instance;
        }
    }
}
