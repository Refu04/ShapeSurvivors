using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spin : Skill
{
    public Spin(int id, Sprite icon, PlayerCore playerInstance) : base(id, icon, playerInstance)
    {
        //名前と説明文
        _name = "Spin";
        _explanation = "回転できるようになる";
        //クールタイム設定
        _maxCoolTime = 0;
        _coolTime = _maxCoolTime;
    }

    public override void Fire()
    {
        base.Fire();
        _playerInstance.transform.Rotate(0, 0, 80 * Time.deltaTime);
    }
}
