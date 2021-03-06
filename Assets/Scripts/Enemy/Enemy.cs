using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

public class Enemy : MonoBehaviour
{
    //HP
    [SerializeField]
    private IntReactiveProperty _hp;

    public int HP
    {
        get { return _hp.Value; }
        set { _hp.Value = value; }
    }

    //Speed
    [SerializeField]
    private float _speed;

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    //寿命がつきたことを表すUniTask
    public UniTask deadAsync => _deadTask.Task;
    private UniTaskCompletionSource _deadTask;
    
    public void Init(int hp, float speed)
    {
        _deadTask = new UniTaskCompletionSource();
        _hp.Value = hp;
        _speed = speed;
    }

    private void Start()
    {
        //HPが０以下になると死亡する
        _hp.Where(x => x <= 0)
           .Subscribe(_ => _deadTask.TrySetResult());
    }

    private void Update()
    {
        //Playerに向かって進み続けるように
        var dir = GameManager.Instance.PlayerCore.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x);
        transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
        transform.position += transform.right * _speed;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameManager.Instance.PlayerCore.HP -= 1;
        }
    }
}
