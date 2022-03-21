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

    //移動スピード
    [SerializeField]
    private float _moveSpeed;

    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    //入力イベント
    private InputEventProvider _inputEventProvider;

    async void Start()
    {
        _inputEventProvider = GetComponent<InputEventProvider>();

        // CancellationTokenSourceを生成
        var cts = new CancellationTokenSource();

        while (true)
        {
            await Move(cts);
        }
    }

    async UniTask Move(CancellationTokenSource cts)
    {
        //FixedUpdateに切り替える
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cts.Token);

        //移動処理
        _inputEventProvider.MoveDirection
            //初期値は無視する
            .Skip(1)
            //移動
            .Subscribe(x => transform.position += x * _moveSpeed);

        //移動量が０で無ければ
        if(_inputEventProvider.MoveDirection.Value != Vector3.zero)
        {
            //移動するのを待つ
            await UniTask.WaitUntilValueChanged(transform, x => x.position);
        }
        

    }
}
