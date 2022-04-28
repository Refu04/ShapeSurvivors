using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class PlayerCore : MonoBehaviour
{
    //レベル（頂点の数）
    [SerializeField]
    private IntReactiveProperty _level;

    public IReadOnlyReactiveProperty<int> Level => _level;

    //所有経験値
    private IntReactiveProperty _exp = new IntReactiveProperty();

    public int EXP
    {
        get { return _exp.Value; }
        set { _exp.Value = value; }
    }

    //次のレベルに上がるのに必要な経験値
    private float _nextLevelEXP;

    //HP
    [SerializeField]
    private int _maxHP;

    private IntReactiveProperty _hp = new IntReactiveProperty();

    public int HP
    {
        get { return _hp.Value; }
        set { _hp.Value = value; }
    }

    //移動スピード
    [SerializeField]
    private float _moveSpeed;

    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    //プレイヤーのスプライト
    [SerializeField]
    private Sprite[] _playerSprites;
    
    //HPバー
    private Image _hpBar;

    //EXPバー
    private Image _expBar;
    
    //弾
    [SerializeField]
    private Bullet _bullet;

    //弾をまとめるオブジェクト
    private Transform _bulletTransform;
    //弾をプールするやつ
    private BulletPool _bulletPool;
    //スキル
    private Skill _firstSkill;
    public Skill FirstSkill
    {
        get { return _firstSkill; }
        set { _firstSkill = value; }
    }
    private Skill _secondSkill;
    public Skill SecondSkill
    {
        get { return _secondSkill; }
        set { _secondSkill = value; }
    }
    private Skill _thirdSkill;
    public Skill ThirdSkill
    {
        get { return _thirdSkill; }
        set { _thirdSkill = value; }
    }
    [SerializeField]
    private Sprite _initIcon;
    //入力イベント
    private InputEventProvider _inputEventProvider;

    //初期化が完了したことを表すUniTask
    public UniTask InitializedAsync => _iniTask.Task;
    private readonly UniTaskCompletionSource _iniTask = new UniTaskCompletionSource();
    //死んだことを表すUniTask
    public UniTask DeadAsync => _deadTask.Task;
    private readonly UniTaskCompletionSource _deadTask = new UniTaskCompletionSource();

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
        _hp.AddTo(this);
        //カメラがプレイヤーについてくるようにする
        this.UpdateAsObservable()
            .Subscribe(_ => Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10)).AddTo(this);
        //HPゲージがプレイヤーについてくるようにする
        _hpBar = GameObject.FindGameObjectWithTag("HPBar").GetComponent<Image>();
        this.UpdateAsObservable()
            .Subscribe(_ =>_hpBar.transform.position =  Camera.main.WorldToScreenPoint(transform.position) - new Vector3(0, 30, 0)).AddTo(this);
        //HP初期化
        _hp.Value = _maxHP;
        //HPが0以下になれば死亡する
        _hp.Where(x => x <= 0)
            .Subscribe(_ => _deadTask.TrySetResult()).AddTo(this);
        //HPとHPバー連携
        _hp.Subscribe(x => {
            _hpBar.fillAmount = (float)x / (float)_maxHP;
        }).AddTo(this);
        //EXPとEXPバー連携
        _expBar = GameObject.FindGameObjectWithTag("EXPBar").GetComponent<Image>();
        _exp.Subscribe(x => {
            _expBar.fillAmount = x / _nextLevelEXP;
        }).AddTo(this);
        //レベルアップ処理
        _exp.Value = 0;
        _nextLevelEXP = 10;
        _exp.Where(x => x >= _nextLevelEXP && _level.Value < _playerSprites.Length)
            .Subscribe(_ =>
            {
                _level.Value += 1;
                _nextLevelEXP *= 3f;
                GetComponent<SpriteRenderer>().sprite = _playerSprites[_level.Value - 3];
                _exp.Value = 0;
            }).AddTo(this);
        //スキル初期化
        _firstSkill = new Skill(99, _initIcon, this);
        _secondSkill = new Skill(99, _initIcon, this);
        _thirdSkill = new Skill(99, _initIcon, this);
        //スキル入力処理
        _inputEventProvider.FirstSkill
                           .Subscribe(_ => _firstSkill.Fire());
        _inputEventProvider.SecondSkill
                           .Subscribe(_ => _secondSkill.Fire());
        _inputEventProvider.ThirdSkill
                           .Subscribe(_ => _thirdSkill.Fire());
        //オブジェクトプールを生成
        _bulletPool = new BulletPool(_bulletTransform, _bullet);
        //破棄されたときにPoolを解放する
        this.OnDestroyAsObservable().Subscribe(_ => _bulletPool.Dispose()).AddTo(this);

        await UniTask.Yield();

        //初期化が終わったらUniTaskを完了させる
        _iniTask.TrySetResult();
    }

    private void OnDestroy()
    {
        //破壊されたらキャンセル
        _iniTask.TrySetCanceled();
        _deadTask.TrySetCanceled();
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
        
        while (true)
        {
            await UniTask.Delay(300);
            for (int i = 0; i < _level.Value; i++)
            {
                Shot(token, i).Forget();
            }
        }
    }

    async UniTask Shot(CancellationToken token, int num)
    {
        
        //プールから弾を１つ取得
        var bullet = _bulletPool.Rent();
        //弾を撃つ
        bullet.Shot(transform.position, transform.localEulerAngles + new Vector3(0, 0, 360 / _level.Value * num));
        //弾が死ぬのを待つ
        await bullet.deadAsync;
        //弾の返却
        _bulletPool.Return(bullet);
        
    }
}
