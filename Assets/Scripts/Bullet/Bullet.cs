using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Bullet : MonoBehaviour
{
    //ダメージ
    [SerializeField]
    private int _damage;

    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    //スピード
    [SerializeField]
    private float _speed;

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    //弾の生存時間
    [SerializeField]
    private float _lifeTime;

    public float LifeTime
    {
        get { return _lifeTime; }
        set { _lifeTime = value; }
    }

    //弾の寿命がつきたことを表すUniTask
    public UniTask deadAsync => _utc.Task;
    private UniTaskCompletionSource _utc;

    //生存時間カウントのDisposable
    SingleAssignmentDisposable disposable;

    public void Shot(Vector3 pos, Vector3 angle)
    {
        //Task生成
        _utc = new UniTaskCompletionSource();
        //弾の位置をセットする
        transform.position = pos;
        //弾の向きをセットする
        transform.localEulerAngles = angle;
        //弾の生存時間をセットする
        var lt = _lifeTime;
        //IDisposableをキャプチャする
        disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.FixedUpdateAsObservable()
            .Subscribe(_ =>
            {
                //生存時間のカウント
                lt -= Time.deltaTime;
                //前に進む
                transform.position += transform.up * _speed;
                //生存時間が尽きれば
                if (lt <= 0)
                {
                    Dead();
                }
            });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Enemy enemy))
        {
            enemy.HP -= _damage;
            Dead();
        }
    }
    
    private void Dead()
    {
        //弾の寿命が尽きたことを伝える
        _utc.TrySetResult();
        //購読破棄
        disposable.Dispose();
    }
}
