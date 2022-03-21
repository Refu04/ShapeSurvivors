using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class InputEventProvider : MonoBehaviour
{
    public IReadOnlyReactiveProperty<Vector3> MoveDirection => _move;

    private readonly ReactiveProperty<Vector3> _move = new ReactiveProperty<Vector3>();

    void Start()
    {
        //OnDestroy時にDisposeされるようにする
        _move.AddTo(this);
    }

    private void Update()
    {
        //移動入力をベクトルとして反映
        //値に変化がなくても強制的に通知する
        _move.SetValueAndForceNotify(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0));
    }
}
