using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
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

    //HP
    [SerializeField]
    private IntReactiveProperty _hp;

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
    //�������������������Ƃ�\��UniTask
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
        //HP��0�ȉ��ɂȂ�Ύ��S����
        _hp.Where(x => x <= 0)
            .Subscribe(_ => _deadTask.TrySetResult());
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
