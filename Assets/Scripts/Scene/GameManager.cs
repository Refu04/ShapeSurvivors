using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class GameManager : SingletonGameObject<GameManager>
{
    //UI
    //ゲームスタートボタン
    private Button _startButton;

    //プレイヤーのプレファブ
    [SerializeField]
    private GameObject _playerPrefab;

    //プレイヤーのインスタンス
    private PlayerCore _playerCore; 

    async void Start()
    {
        // CancellationTokenSourceを生成
        var token = this.GetCancellationTokenOnDestroy();
        await GameFlow(token);
    }

    public async UniTask GameFlow(CancellationToken token)
    {
        while (true)
        {
            await Title(token);
            await Game(token);
        }
    }

    public async UniTask Title(CancellationToken token)
    {
        //Titleシーンを読み込む
        await SceneManager.LoadSceneAsync("Title");
        Debug.Log("Titleシーンがロードされた");
        //Titleシーンの初期化
        await InitializeTitle();
        //ゲームスタートボタンが押されるまで待つ
        await _startButton.OnClickAsync(token);
        
    }

    public async UniTask Game(CancellationToken token)
    {
        //Gameシーンを読み込む
        await SceneManager.LoadSceneAsync("Game");
        Debug.Log("Gameシーンがロードされた");
        //Gameシーンの初期化
        await InitializeGame();
        //Playerが死亡するまで待つ
        await UniTask.WaitUntil(() => _playerCore.HP <= 0);
        //リザルト表示

        //ゲームが終わるまで待つ
    }

    public async UniTask InitializeTitle()
    {
        //スタートボタンの割当
        Debug.Log("Titleシーン初期化");
        _startButton = GameObject.FindGameObjectWithTag("StartButton").GetComponent<Button>();
        await UniTask.WaitUntil(() => _startButton != null);
    }

    public async UniTask InitializeGame()
    {
        Debug.Log("Gameシーン初期化");
        //プレイヤーの生成
        var player = Instantiate(_playerPrefab);
        //プレイヤーのインスタンスをキャッシュ
        _playerCore = player.GetComponent<PlayerCore>();
        //プレイヤーの初期化待ち
        await _playerCore.InitializedAsync;
    }
}

