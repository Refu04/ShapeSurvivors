using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class InputEventProvider : MonoBehaviour
{
    public IReadOnlyReactiveProperty<Vector3> MoveDirection => _move;
    public IObservable<Unit> FirstSkill => _firstSkill;
    public IObservable<Unit> SecondSkill => _secondSkill;
    public IObservable<Unit> ThirdSkill => _thirdSkill;

    private readonly ReactiveProperty<Vector3> _move = new ReactiveProperty<Vector3>();
    private readonly Subject<Unit> _firstSkill = new Subject<Unit>();
    private readonly Subject<Unit> _secondSkill = new Subject<Unit>();
    private readonly Subject<Unit> _thirdSkill = new Subject<Unit>();

    void Start()
    {
        //OnDestroy時にDisposeされるようにする
        _move.AddTo(this);
        _firstSkill.AddTo(this);
        _secondSkill.AddTo(this);
        _thirdSkill.AddTo(this);
    }

    private void Update()
    {
        //移動入力をベクトルとして反映
        //値に変化がなくても強制的に通知する
        _move.SetValueAndForceNotify(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0));

        //スキル入力
        if(Input.GetKey(KeyCode.Q))
        {
            _firstSkill.OnNext(Unit.Default);
        }
        if (Input.GetKey(KeyCode.E))
        {
            _secondSkill.OnNext(Unit.Default);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _thirdSkill.OnNext(Unit.Default);
        }
    }
}
