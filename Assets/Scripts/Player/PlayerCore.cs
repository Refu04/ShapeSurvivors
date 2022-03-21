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

    //�������������������Ƃ�\��UniTask
    public UniTask InitializedAsync => _utc.Task;
    private readonly UniTaskCompletionSource _utc = new UniTaskCompletionSource();

    async void Start()
    {
        //����������
        await InitializeAsync();

        // CancellationTokenSource�𐶐�
        var token = this.GetCancellationTokenOnDestroy();

        while (true)
        {
            await Move(token);
        }
    }

    //������
    private async UniTask InitializeAsync()
    {
        _inputEventProvider = GetComponent<InputEventProvider>();

        await UniTask.Yield();

        //���������I�������UniTask������������
        _utc.TrySetResult();
    }

    private void OnDestroy()
    {
        //�j�󂳂ꂽ��L�����Z��
        _utc.TrySetCanceled();
    }

    async UniTask Move(CancellationToken token)
    {
        //FixedUpdate�ɐ؂�ւ���
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);

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
