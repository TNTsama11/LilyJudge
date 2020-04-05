using System;
using System.Collections.Generic;
using UnityEngine;

public class ShowToast
{
    /// <summary>
    /// 显示提示信息 Android显示toast 
    /// </summary>
    public static void MakeToast(string info)
    {
#if UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            Toast.CallStatic<AndroidJavaObject>("makeText", currentActivity, info, Toast.GetStatic<int>("LENGTH_LONG")).Call("show");
        }));
#elif UNITY_EDITOR
        Debug.Log(info);
#endif
    }
}

