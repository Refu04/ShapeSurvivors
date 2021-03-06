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
        //新しく生成
        var b = GameObject.Instantiate(_bulletPrefab);
        //Hieralchy上で散らからないようにする
        b.transform.SetParent(_parenTransform);

        return b;
    }
}
