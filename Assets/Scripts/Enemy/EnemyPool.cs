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
        //新しく生成
        var e = GameObject.Instantiate(_enemyPrefab);
        //Hieralchy上で散らからないようにする
        e.transform.SetParent(_parenTransform);

        return e;
    }
}
