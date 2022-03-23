using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Bullet : MonoBehaviour
{
    //�_���[�W
    [SerializeField]
    private int _damage;

    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    //�X�s�[�h
    [SerializeField]
    private float _speed;

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    //�e�̐�������
    [SerializeField]
    private float _lifeTime;

    public float LifeTime
    {
        get { return _lifeTime; }
        set { _lifeTime = value; }
    }

    //�e�̎������������Ƃ�\��UniTask
    public UniTask deadAsync => _utc.Task;
    private UniTaskCompletionSource _utc;

    public void Shot(Vector3 pos, Vector3 angle)
    {
        //Task����
        _utc = new UniTaskCompletionSource();
        //�e�̈ʒu���Z�b�g����
        transform.position = pos;
        //�e�̌������Z�b�g����
        transform.localEulerAngles = angle;
        //�e�̐������Ԃ��Z�b�g����
        var lt = _lifeTime;
        //IDisposable���L���v�`������
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.FixedUpdateAsObservable()
            .Subscribe(_ =>
            {
                //�������Ԃ̃J�E���g
                lt -= Time.deltaTime;
                //�O�ɐi��
                transform.position += transform.up * _speed;
                //�������Ԃ��s�����
                if (lt <= 0)
                {
                    //�e�̎������s�������Ƃ�`����
                    _utc.TrySetResult();
                    //�w�ǔj��
                    disposable.Dispose();
                }
            });
    }
}
