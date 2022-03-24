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
    public UniTask InitializedAsync => _utc.Task;
    private readonly UniTaskCompletionSource _utc = new UniTaskCompletionSource();

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
        //�I�u�W�F�N�g�v�[���𐶐�
        _bulletPool = new BulletPool(_bulletTransform, _bullet);
        //�j�����ꂽ�Ƃ���Pool���������
        this.OnDestroyAsObservable().Subscribe(_ => _bulletPool.Dispose());

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
        //�v�[������e���P�擾
        var bullet = _bulletPool.Rent();
        //�e������
        bullet.Shot(transform.position, transform.localEulerAngles);
        //�e�����ʂ̂�҂�
        await bullet.deadAsync;
        //�e�̕ԋp
        _bulletPool.Return(bullet);
    }
}
