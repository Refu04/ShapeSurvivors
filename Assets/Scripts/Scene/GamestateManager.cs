using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class GamestateManager : MonoBehaviour
{
    
    private Button m_startButton;

    async void Start()
    {
        // CancellationTokenSource�𐶐�
        var cts = new CancellationTokenSource();
        await GameFlow(cts.Token);
    }

    public void InitializeTitle()
    {
        //�X�^�[�g�{�^���̊���
        Debug.Log("Title�V�[��������");
        m_startButton = GameObject.FindGameObjectWithTag("StartButton").GetComponent<Button>();
       
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
        //Title�V�[����ǂݍ���
        await SceneManager.LoadSceneAsync("Title");
        Debug.Log("Title�V�[�������[�h���ꂽ");
        //Title�V�[���̏�����
        InitializeTitle();
        //�Q�[���X�^�[�g�{�^�����������܂ő҂�
        await m_startButton.OnClickAsync(cts);
        
    }

    public async UniTask Game(CancellationToken cts)
    {
        //Game�V�[����ǂݍ���
        await SceneManager.LoadSceneAsync("Game");
        Debug.Log("Game�V�[�������[�h���ꂽ");
        //�Q�[�����I���܂ő҂�
        await UniTask.Delay(2000);
    }
}

