using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Toolkit;

public class BulletPool : ObjectPool<Bullet>
{
    private readonly Bullet _bulletPrefab;
    private readonly Transform _parenTransform;

    public BulletPool(Transform parenTransform, Bullet prefab)
    {
        _parenTransform = parenTransform;
        _bulletPrefab = prefab;
    }

    protected override Bullet CreateInstance()
    {
        //�V��������
        var b = GameObject.Instantiate(_bulletPrefab);
        //Hieralchy��ŎU�炩��Ȃ��悤�ɂ���
        b.transform.SetParent(_parenTransform);

        return b;
    }
}
