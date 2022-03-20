using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class GamestateManager : MonoBehaviour
{
    [SerializeField]
    private Button m_startButton;

    public async UniTask GameFlow()
    {
        while (true)
        {
            await Title();
            await Game();
        }
    }

    public async UniTask Title()
    {
        //ゲームスタートボタンが押されるまで待つ
    }

    public async UniTask Game()
    {
        //ゲームが終わるまで待つ
    }
}

