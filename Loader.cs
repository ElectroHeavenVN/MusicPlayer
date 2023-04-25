using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Loader
{
    static GameObject _Load;

    public static void Init()
    {
        _Load = new GameObject();
        _Load.AddComponent<MainExt>();
        UnityEngine.Object.DontDestroyOnLoad(_Load);
    }
}