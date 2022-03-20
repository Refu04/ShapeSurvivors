using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DontDestroyOnLoadなオブジェクトに付ける
public class SingletonGameObject : MonoBehaviour
{
    private static GameObject mInstance;

    public static GameObject Instance
    {
        get
        {
            return mInstance;
        }
    }

    void Awake()
    {
        //自分のインスタンスが登録されているかチェック
        if (mInstance == null)
        {
            //無ければ破壊されないようにし、登録
            DontDestroyOnLoad(gameObject);
            mInstance = gameObject;
        }
        else
        {
            //既にインスタンスがあれば破壊する
            Destroy(gameObject);
        }
    }
}