using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spin : Skill
{
    public Spin(int id, Sprite icon, PlayerCore playerInstance) : base(id, icon, playerInstance)
    {
        //���O�Ɛ�����
        _name = "Spin";
        _explanation = "��]�ł���悤�ɂȂ�";
        //�N�[���^�C���ݒ�
        _maxCoolTime = 0;
        _coolTime = _maxCoolTime;
    }

    public override void Fire()
    {
        base.Fire();
        _playerInstance.transform.Rotate(0, 0, 80 * Time.deltaTime);
    }
}
