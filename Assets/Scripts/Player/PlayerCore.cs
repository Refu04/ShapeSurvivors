using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

public class PlayerCore : MonoBehaviour
{
    //HP
    [SerializeField]
    private int _hp;

    public int HP
    {
        get { return _hp; }
        set { _hp = value; }
    }

    //�ړ��X�s�[�h
    [SerializeField]
    private float _moveSpeed;

    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    //���̓C�x���g
    private InputEventProvider _inputEventProvider;

    async void Start()
    {
        _inputEventProvider = GetComponent<InputEventProvider>();

        // CancellationTokenSource�𐶐�
        var cts = new CancellationTokenSource();

        while (true)
        {
            await Move(cts);
        }
    }

    async UniTask Move(CancellationTokenSource cts)
    {
        //FixedUpdate�ɐ؂�ւ���
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cts.Token);

        //�ړ�����
        _inputEventProvider.MoveDirection
            //�����l�͖�������
            .Skip(1)
            //�ړ�
            .Subscribe(x => transform.position += x * _moveSpeed);

        //�ړ��ʂ��O�Ŗ������
        if(_inputEventProvider.MoveDirection.Value != Vector3.zero)
        {
            //�ړ�����̂�҂�
            await UniTask.WaitUntilValueChanged(transform, x => x.position);
        }
        

    }
}
