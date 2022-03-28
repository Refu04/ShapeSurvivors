using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Enemy _enemyPrefab;
    [SerializeField]
    private Gem _gemPrefab;
    [SerializeField]
    private EnemyParam[] _enemyParams;
    [SerializeField]
    private GemParam[] _gemParams;

    private float _time;
    //Enemyをまとめるオブジェクト
    [SerializeField]
    private Transform _enemyTransform;
    //Enemyをプールするやつ
    private EnemyPool _enemyPool;
    //Gemをまとめるオブジェクト
    [SerializeField]
    private Transform _gemTransform;
    //Gemをプールするやつ
    private GemPool _gemPool;
    // Update is called once per frame
    async void Start()
    {
        //敵オブジェクトのプールを作成
        _enemyPool = new EnemyPool(_enemyTransform, _enemyPrefab);
        //Gemオブジェクトのプールを作成
        _gemPool = new GemPool(_gemTransform, _gemPrefab);
        var token = this.GetCancellationTokenOnDestroy();
        //FixedUpdateに切り替える
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
        while (true)
        {
            _time += Time.deltaTime;
            if (_time > 0)
            {
                await UniTask.Delay(800);
                InstantiateEnemy(_enemyPrefab, _enemyParams[0]).Forget();
            }
        }
        
    }

    async UniTask InstantiateEnemy(Enemy enemyPrefab, EnemyParam param)
    {
        var playerPos = GameManager.Instance.PlayerCore.transform.position;
        //画面外にランダムに生成されるようにする
        var enemy = _enemyPool.Rent();
        var random = Random.Range(0, 4);
        var pos = Vector3.zero;
        switch (random)
        {
            case 0:
                pos = new Vector3(playerPos.x + 15, playerPos.y + Random.Range(-15, 15), 0);
                break;
            case 1:
                pos = new Vector3(playerPos.x - 15, playerPos.y + Random.Range(-15, 15), 0);
                break;
            case 2:
                pos = new Vector3(playerPos.x + Random.Range(-15, 15), playerPos.y + 15, 0);
                break;
            case 3:
                pos = new Vector3(playerPos.x + Random.Range(-15, 15), playerPos.y - 15, 0);
                break;
        }
        enemy.transform.position = pos;
        //Enemy初期化
        enemy.Init(param.HP, param.speed);
        //Enemyが死ぬのを待つ
        await enemy.deadAsync;
        //Gemの生成
        InstantiateGem(enemy.transform.position, _gemParams[0]).Forget();
        //死んだら返却
        _enemyPool.Return(enemy);
    }

    async UniTask InstantiateGem(Vector3 pos, GemParam param)
    {
        var gem = _gemPool.Rent();
        gem.Init(pos, param);
        await gem.DeadAsync;
        _gemPool.Return(gem);
    }
}
