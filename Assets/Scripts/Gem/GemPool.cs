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
        //êVÇµÇ≠ê∂ê¨
        var g = GameObject.Instantiate(_gemPrefab);
        //Hieralchyè„Ç≈éUÇÁÇ©ÇÁÇ»Ç¢ÇÊÇ§Ç…Ç∑ÇÈ
        g.transform.SetParent(_parenTransform);

        return g;
    }
}
