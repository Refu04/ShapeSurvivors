using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DontDestroyOnLoad�ȃI�u�W�F�N�g�ɕt����
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
        //�����̃C���X�^���X���o�^����Ă��邩�`�F�b�N
        if (mInstance == null)
        {
            //������Δj�󂳂�Ȃ��悤�ɂ��A�o�^
            DontDestroyOnLoad(gameObject);
            mInstance = gameObject;
        }
        else
        {
            //���ɃC���X�^���X������Δj�󂷂�
            Destroy(gameObject);
        }
    }
}