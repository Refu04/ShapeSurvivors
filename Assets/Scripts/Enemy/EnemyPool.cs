using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Toolkit;

public class EnemyPool : ObjectPool<Enemy>
{
    private readonly Enemy _enemyPrefab;
    private readonly Transform _parenTransform;

    public EnemyPool(Transform parenTransform, Enemy prefab)
    {
        _parenTransform = parenTransform;
        _enemyPrefab = prefab;
    }

    protected override Enemy CreateInstance()
    {
        //êVÇµÇ≠ê∂ê¨
        var e = GameObject.Instantiate(_enemyPrefab);
        //Hieralchyè„Ç≈éUÇÁÇ©ÇÁÇ»Ç¢ÇÊÇ§Ç…Ç∑ÇÈ
        e.transform.SetParent(_parenTransform);

        return e;
    }
}
