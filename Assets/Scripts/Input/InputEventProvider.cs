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
        //OnDestroy����Dispose�����悤�ɂ���
        _move.AddTo(this);
    }

    private void Update()
    {
        //�ړ����͂��x�N�g���Ƃ��Ĕ��f
        //�l�ɕω����Ȃ��Ă������I�ɒʒm����
        _move.SetValueAndForceNotify(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0));
    }
}
