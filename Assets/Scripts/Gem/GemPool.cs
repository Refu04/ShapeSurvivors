using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Toolkit;

public class GemPool : ObjectPool<Gem>
{
    private readonly Gem _gemPrefab;
    private readonly Transform _parenTransform;

    public GemPool(Transform parenTransform, Gem prefab)
    {
        _parenTransform = parenTransform;
        _gemPrefab = prefab;
    }

    protected override Gem CreateInstance()
    {
        //新しく生成
        var g = GameObject.Instantiate(_gemPrefab);
        //Hieralchy上で散らからないようにする
        g.transform.SetParent(_parenTransform);

        return g;
    }
}
