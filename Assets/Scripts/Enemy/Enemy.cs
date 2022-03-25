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

    //õ–½‚ª‚Â‚«‚½‚±‚Æ‚ğ•\‚·UniTask
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
        //HP‚ª‚OˆÈ‰º‚É‚È‚é‚Æ€–S‚·‚é
        _hp.Where(x => x <= 0)
           .Subscribe(_ => _deadTask.TrySetResult());
    }

    private void Update()
    {
        //Player‚ÉŒü‚©‚Á‚Äi‚İ‘±‚¯‚é‚æ‚¤‚É
        //transform.LookAt(GameManager.Instance.PlayerCore.transform);
        //transform.position += transform.up * _speed;
    }
}
