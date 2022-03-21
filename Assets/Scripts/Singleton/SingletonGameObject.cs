using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�V���O���g���ȃN���X�Ɍp��������
public class SingletonGameObject<T> : MonoBehaviour where T :SingletonGameObject<T>
{
    private static T mInstance;

    public static T Instance
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
            DontDestroyOnLoad(this.gameObject);
            mInstance = this as T;
            mInstance.Init();
        }
        else
        {
            //���ɃC���X�^���X������Δj�󂷂�
            Destroy(this.gameObject);
        }
    }

    //�h���N���X�p��Awake
    protected virtual void Init()
    {

    }
}