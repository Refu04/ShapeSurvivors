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
    //�Q�[���X�^�[�g�{�^��
    private Button _startButton;

    //�v���C���[�̃C���X�^���X
    private PlayerCore _playerCore;

    async void Start()
    {
        // CancellationTokenSource�𐶐�
        var cts = new CancellationTokenSource();
        await GameFlow(cts.Token);
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
        await _startButton.OnClickAsync(cts);
        
    }

    public async UniTask Game(CancellationToken cts)
    {
        //Game�V�[����ǂݍ���
        await SceneManager.LoadSceneAsync("Game");
        Debug.Log("Game�V�[�������[�h���ꂽ");
        //Game�V�[���̏�����
        InitializeGame();
        //Player�����S����܂ő҂�
        await UniTask.WaitUntil(() => _playerCore.HP <= 0);
        //���U���g�\��

        //�Q�[�����I���܂ő҂�
    }

    public void InitializeTitle()
    {
        //�X�^�[�g�{�^���̊���
        Debug.Log("Title�V�[��������");
        _startButton = GameObject.FindGameObjectWithTag("StartButton").GetComponent<Button>();

    }

    public void InitializeGame()
    {
        Debug.Log("Game�V�[��������");
        _playerCore = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCore>();
    }
}

