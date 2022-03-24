using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerCore : MonoBehaviour
{
    //レベル（頂点の数）
    [SerializeField]
    private int _level;

    public int Level
    {
        get { return _level; }
        set { _level = value; }
    }

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

    //弾
    [SerializeField]
    private Bullet _bullet;

    //弾をまとめるオブジェクト
    private Transform _bulletTransform;
    //弾をプールするやつ
    private BulletPool _bulletPool;

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
        //弾の射撃を開始する
        RapidFire(token).Forget();
        while (true)
        {
            await Move(token);
        }
    }

    //初期化
    private async UniTask InitializeAsync()
    {
        _inputEventProvider = GetComponent<InputEventProvider>();
        _bulletTransform = GameObject.FindGameObjectWithTag("BulletTransform").transform;
        //オブジェクトプールを生成
        _bulletPool = new BulletPool(_bulletTransform, _bullet);
        //破棄されたときにPoolを解放する
        this.OnDestroyAsObservable().Subscribe(_ => _bulletPool.Dispose());

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
        transform.position += _inputEventProvider.MoveDirection.Value * _moveSpeed;
    }

    async UniTask RapidFire(CancellationToken token)
    {
        while(true)
        {
            await UniTask.Delay(100);
            Shot(token).Forget();
        }
    }

    async UniTask Shot(CancellationToken token)
    {
        //プールから弾を１つ取得
        var bullet = _bulletPool.Rent();
        //弾を撃つ
        bullet.Shot(transform.position, transform.localEulerAngles);
        //弾が死ぬのを待つ
        await bullet.deadAsync;
        //弾の返却
        _bulletPool.Return(bullet);
    }
}
