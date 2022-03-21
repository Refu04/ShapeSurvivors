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
    private Button m_startButton;
    

    async void Start()
    {
        // CancellationTokenSourceを生成
        var cts = new CancellationTokenSource();
        await GameFlow(cts.Token);
    }

    public void InitializeTitle()
    {
        //スタートボタンの割当
        Debug.Log("Titleシーン初期化");
        m_startButton = GameObject.FindGameObjectWithTag("StartButton").GetComponent<Button>();
       
    }

    public void InitializeGame()
    {
        
    }

    public async UniTask GameFlow(CancellationToken cts)
    {
        while (true)
        {
            await Title(cts);
            await Game(cts);
        }
    }

    public async UniTask Title(CancellationToken cts)
    {
        //Titleシーンを読み込む
        await SceneManager.LoadSceneAsync("Title");
        Debug.Log("Titleシーンがロードされた");
        //Titleシーンの初期化
        InitializeTitle();
        //ゲームスタートボタンが押されるまで待つ
        await m_startButton.OnClickAsync(cts);
        
    }

    public async UniTask Game(CancellationToken cts)
    {
        //Gameシーンを読み込む
        await SceneManager.LoadSceneAsync("Game");
        Debug.Log("Gameシーンがロードされた");
        //Gameシーンの初期化
        InitializeGame();
        //ゲームが終わるまで待つ
        await UniTask.Delay(2000);
    }
}

