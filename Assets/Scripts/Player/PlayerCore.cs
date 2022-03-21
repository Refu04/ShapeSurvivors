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

    //初期化が完了したことを表すUniTask
    public UniTask InitializedAsync => _utc.Task;
    private readonly UniTaskCompletionSource _utc = new UniTaskCompletionSource();

    async void Start()
    {
        //初期化処理
        await InitializeAsync();

        // CancellationTokenSourceを生成
        var token = this.GetCancellationTokenOnDestroy();

        while (true)
        {
            await Move(token);
        }
    }

    //初期化
    private async UniTask InitializeAsync()
    {
        _inputEventProvider = GetComponent<InputEventProvider>();

        await UniTask.Yield();

        //初期化が終わったらUniTaskを完了させる
        _utc.TrySetResult();
    }

    private void OnDestroy()
    {
        //破壊されたらキャンセル
        _utc.TrySetCanceled();
    }

    async UniTask Move(CancellationToken token)
    {
        //FixedUpdateに切り替える
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);

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
