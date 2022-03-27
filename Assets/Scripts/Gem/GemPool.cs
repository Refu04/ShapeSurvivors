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
        //�V��������
        var g = GameObject.Instantiate(_gemPrefab);
        //Hieralchy��ŎU�炩��Ȃ��悤�ɂ���
        g.transform.SetParent(_parenTransform);

        return g;
    }
}
