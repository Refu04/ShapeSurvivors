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
    //Enemy���܂Ƃ߂�I�u�W�F�N�g
    [SerializeField]
    private Transform _enemyTransform;
    //Enemy���v�[��������
    private EnemyPool _enemyPool;
    //Gem���܂Ƃ߂�I�u�W�F�N�g
    [SerializeField]
    private Transform _gemTransform;
    //Gem���v�[��������
    private GemPool _gemPool;
    // Update is called once per frame
    async void Start()
    {
        //�G�I�u�W�F�N�g�̃v�[�����쐬
        _enemyPool = new EnemyPool(_enemyTransform, _enemyPrefab);
        //Gem�I�u�W�F�N�g�̃v�[�����쐬
        _gemPool = new GemPool(_gemTransform, _gemPrefab);
        var token = this.GetCancellationTokenOnDestroy();
        //FixedUpdate�ɐ؂�ւ���
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
        //��ʊO�Ƀ����_���ɐ��������悤�ɂ���
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
        //Enemy������
        enemy.Init(param.HP, param.speed);
        //Enemy�����ʂ̂�҂�
        await enemy.deadAsync;
        //Gem�̐���
        InstantiateGem(enemy.transform.position, _gemParams[0]).Forget();
        //���񂾂�ԋp
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
