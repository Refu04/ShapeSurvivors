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
    //���x���i���_�̐��j
    [SerializeField]
    private IntReactiveProperty _level;

    public IReadOnlyReactiveProperty<int> Level => _level;

    //���L�o���l
    private IntReactiveProperty _exp = new IntReactiveProperty();

    public int EXP
    {
        get { return _exp.Value; }
        set { _exp.Value = value; }
    }

    //���̃��x���ɏオ��̂ɕK�v�Ȍo���l
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

    //�ړ��X�s�[�h
    [SerializeField]
    private float _moveSpeed;

    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    //�v���C���[�̃X�v���C�g
    [SerializeField]
    private Sprite[] _playerSprites;
    
    //HP�o�[
    private Image _hpBar;

    //EXP�o�[
    private Image _expBar;
    
    //�e
    [SerializeField]
    private Bullet _bullet;

    //�e���܂Ƃ߂�I�u�W�F�N�g
    private Transform _bulletTransform;
    //�e���v�[��������
    private BulletPool _bulletPool;
    //�X�L��
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
    //���̓C�x���g
    private InputEventProvider _inputEventProvider;

    //�������������������Ƃ�\��UniTask
    public UniTask InitializedAsync => _iniTask.Task;
    private readonly UniTaskCompletionSource _iniTask = new UniTaskCompletionSource();
    //���񂾂��Ƃ�\��UniTask
    public UniTask DeadAsync => _deadTask.Task;
    private readonly UniTaskCompletionSource _deadTask = new UniTaskCompletionSource();

    async void Start()
    {
        //����������
        await InitializeAsync();

        // CancellationTokenSource�𐶐�
        var token = this.GetCancellationTokenOnDestroy();
        //�e�̎ˌ����J�n����
        RapidFire(token).Forget();
        while (true)
        {
            await Move(token);
        }
    }

    //������
    private async UniTask InitializeAsync()
    {
        _inputEventProvider = GetComponent<InputEventProvider>();
        _bulletTransform = GameObject.FindGameObjectWithTag("BulletTransform").transform;
        _hp.AddTo(this);
        //�J�������v���C���[�ɂ��Ă���悤�ɂ���
        this.UpdateAsObservable()
            .Subscribe(_ => Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10)).AddTo(this);
        //HP�Q�[�W���v���C���[�ɂ��Ă���悤�ɂ���
        _hpBar = GameObject.FindGameObjectWithTag("HPBar").GetComponent<Image>();
        this.UpdateAsObservable()
            .Subscribe(_ =>_hpBar.transform.position =  Camera.main.WorldToScreenPoint(transform.position) - new Vector3(0, 30, 0)).AddTo(this);
        //HP������
        _hp.Value = _maxHP;
        //HP��0�ȉ��ɂȂ�Ύ��S����
        _hp.Where(x => x <= 0)
            .Subscribe(_ => _deadTask.TrySetResult()).AddTo(this);
        //HP��HP�o�[�A�g
        _hp.Subscribe(x => {
            _hpBar.fillAmount = (float)x / (float)_maxHP;
        }).AddTo(this);
        //EXP��EXP�o�[�A�g
        _expBar = GameObject.FindGameObjectWithTag("EXPBar").GetComponent<Image>();
        _exp.Subscribe(x => {
            _expBar.fillAmount = x / _nextLevelEXP;
        }).AddTo(this);
        //���x���A�b�v����
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
        //�X�L��������
        _firstSkill = new Skill(99, _initIcon, this);
        _secondSkill = new Skill(99, _initIcon, this);
        _thirdSkill = new Skill(99, _initIcon, this);
        //�X�L�����͏���
        _inputEventProvider.FirstSkill
                           .Subscribe(_ => _firstSkill.Fire());
        _inputEventProvider.SecondSkill
                           .Subscribe(_ => _secondSkill.Fire());
        _inputEventProvider.ThirdSkill
                           .Subscribe(_ => _thirdSkill.Fire());
        //�I�u�W�F�N�g�v�[���𐶐�
        _bulletPool = new BulletPool(_bulletTransform, _bullet);
        //�j�����ꂽ�Ƃ���Pool���������
        this.OnDestroyAsObservable().Subscribe(_ => _bulletPool.Dispose()).AddTo(this);

        await UniTask.Yield();

        //���������I�������UniTask������������
        _iniTask.TrySetResult();
    }

    private void OnDestroy()
    {
        //�j�󂳂ꂽ��L�����Z��
        _iniTask.TrySetCanceled();
        _deadTask.TrySetCanceled();
    }

    async UniTask Move(CancellationToken token)
    {
        //FixedUpdate�ɐ؂�ւ���
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);

        //�ړ�����
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
        
        //�v�[������e���P�擾
        var bullet = _bulletPool.Rent();
        //�e������
        bullet.Shot(transform.position, transform.localEulerAngles + new Vector3(0, 0, 360 / _level.Value * num));
        //�e�����ʂ̂�҂�
        await bullet.deadAsync;
        //�e�̕ԋp
        _bulletPool.Return(bullet);
        
    }
}
