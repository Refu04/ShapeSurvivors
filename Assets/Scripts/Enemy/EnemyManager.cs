using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Enemy _enemyPrefab;

    private float _time;
    //弾をまとめるオブジェクト
    [SerializeField]
    private Transform _enemyTransform;
    //Enemyをプールするやつ
    private EnemyPool _enemyPool;
    // Update is called once per frame
    async void Start()
    {
        //敵オブジェクトのプールを作成
        _enemyPool = new EnemyPool(_enemyTransform, _enemyPrefab);
        var token = this.GetCancellationTokenOnDestroy();
        //FixedUpdateに切り替える
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
        while (true)
        {
            _time += Time.deltaTime;
            if (_time > 0)
            {
                await UniTask.Delay(800);
                InstantiateEnemy(_enemyPrefab).Forget();
            }
        }
        
    }

    async UniTask InstantiateEnemy(Enemy enemyPrefab)
    {
        var playerPos = GameManager.Instance.PlayerCore.transform.position;
        //画面外にランダムに生成されるようにする
        var enemy = _enemyPool.Rent();
        var random = Random.Range(0, 4);
        var pos = Vector3.zero;
        switch (random)
        {
            case 0:
                pos = new Vector3(playerPos.x + 10, playerPos.y + Random.Range(-10, 10), 0);
                break;
            case 1:
                pos = new Vector3(playerPos.x - 10, playerPos.y + Random.Range(-10, 10), 0);
                break;
            case 2:
                pos = new Vector3(playerPos.x + Random.Range(-10, 10), playerPos.y + 10, 0);
                break;
            case 3:
                pos = new Vector3(playerPos.x + Random.Range(-10, 10), playerPos.y - 10, 0);
                break;
        }
        enemy.transform.position = pos;
        //Enemy初期化
        enemy.Init(10, 0.1f);
        //Enemyが死ぬのを待つ
        await enemy.deadAsync;
        //死んだら返却
        _enemyPool.Return(enemy);
    }
}
