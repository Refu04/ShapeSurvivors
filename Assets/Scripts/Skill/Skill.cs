using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class Skill : MonoBehaviour
{
    public int Id => _id;
    public int Level => _level;
    public float MaxCoolTime => _maxCoolTime;
    public float CoolTime => _coolTime;
    public Sprite Icon => _icon;
    public string Name => _name;
    public string Explanation => _explanation;

    protected int _id;
    protected int _level;
    protected float _maxCoolTime;
    protected float _coolTime;
    protected Sprite _icon;
    protected PlayerCore _playerInstance;
    protected string _name;
    protected string _explanation;

    public Skill(int id, Sprite icon, PlayerCore playerInstance)
    {
        _id = id;
        _level = 1;
        _icon = icon;
        _maxCoolTime = 0;
        _coolTime = 0;
        _name = "";
        _explanation = "";
        _playerInstance = playerInstance;
    }

    public virtual void Fire()
    {
        if(_coolTime == _maxCoolTime)
        {
            Debug.Log("ƒXƒLƒ‹”­“®");
            Recast();
        }
    }

    public void Recast()
    {
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable()
            .Subscribe(_ => {
                _coolTime -= Time.deltaTime;
                if(_coolTime <= 0)
                {
                    _coolTime = _maxCoolTime;
                    disposable.Dispose();
                }
            });
    }

    public void LevelUp()
    {
        _level++;
    }
}
