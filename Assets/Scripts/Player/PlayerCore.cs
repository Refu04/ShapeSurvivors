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
    private int _level;

    public int Level
    {
        get { return _level; }
        set { _level = value; }
    }

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
        Camera.main.transform.parent = transform;
        //HP�Q�[�W���v���C���[�ɂ��Ă���悤�ɂ���
        _hpBar = GameObject.FindGameObjectWithTag("HPBar").GetComponent<Image>();
        this.UpdateAsObservable()
            .Subscribe(_ =>_hpBar.transform.position =  Camera.main.WorldToScreenPoint(transform.position) - new Vector3(0, 30, 0));
        //HP������
        _hp.Value = _maxHP;
        //HP��0�ȉ��ɂȂ�Ύ��S����
        _hp.Where(x => x <= 0)
            .Subscribe(_ => _deadTask.TrySetResult());
        //HP��HP�o�[�A�g
        _hp.Subscribe(x => {
            _hpBar.fillAmount = (float)x / (float)_maxHP;
            Debug.Log(x);
        });
        //EXP��EXP�o�[�A�g
        _expBar = GameObject.FindGameObjectWithTag("EXPBar").GetComponent<Image>();
        _exp.Subscribe(x => {
            _expBar.fillAmount = x / _nextLevelEXP;
        });
        //���x���A�b�v����
        _exp.Value = 0;
        _nextLevelEXP = 10;
        _exp.Where(x => x >= _nextLevelEXP && _level < _playerSprites.Length)
            .Subscribe(_ =>
            {
                _level += 1;
                _nextLevelEXP *= 3f;
                GetComponent<SpriteRenderer>().sprite = _playerSprites[_level - 3];
                _exp.Value = 0;
            });
        //�I�u�W�F�N�g�v�[���𐶐�
        _bulletPool = new BulletPool(_bulletTransform, _bullet);
        //�j�����ꂽ�Ƃ���Pool���������
        this.OnDestroyAsObservable().Subscribe(_ => _bulletPool.Dispose());

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
            for (int i = 0; i < _level; i++)
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
        bullet.Shot(transform.position, new Vector3(0, 0, 360 / _level * num));
        //�e�����ʂ̂�҂�
        await bullet.deadAsync;
        //�e�̕ԋp
        _bulletPool.Return(bullet);
    }
}
