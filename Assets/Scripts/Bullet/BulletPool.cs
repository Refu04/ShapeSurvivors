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
        //êVÇµÇ≠ê∂ê¨
        var b = GameObject.Instantiate(_bulletPrefab);
        //Hieralchyè„Ç≈éUÇÁÇ©ÇÁÇ»Ç¢ÇÊÇ§Ç…Ç∑ÇÈ
        b.transform.SetParent(_parenTransform);

        return b;
    }
}
