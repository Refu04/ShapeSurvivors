using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Gem : MonoBehaviour
{
    private GemParam _param;

    //死んだことを表すUniTask
    public UniTask DeadAsync => _deadTask.Task;
    private UniTaskCompletionSource _deadTask;

    public void Init(Vector3 pos, GemParam param)
    {
        _deadTask = new UniTaskCompletionSource();
        _param = param;
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "GetItem")
        {
            //プレイヤーに向かって進む
            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    var player = GameManager.Instance.PlayerCore;
                    var dir = player.transform.position - transform.position;
                    var angle = Mathf.Atan2(dir.y, dir.x);
                    transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
                    transform.position += transform.right * 0.09f;
                    if(Vector3.Distance(transform.position, player.transform.position) < 0.2f)
                    {
                        //向きを戻す
                        transform.localEulerAngles = Vector3.zero;
                        //プレイヤーに経験値を追加
                        player.EXP += _param.EXP;
                        //死亡処理
                        disposable.Dispose();
                        _deadTask.TrySetResult();

                    }
                });
            
        }

    }


}
