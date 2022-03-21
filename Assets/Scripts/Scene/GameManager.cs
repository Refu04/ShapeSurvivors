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

    //�v���C���[�̃v���t�@�u
    [SerializeField]
    private GameObject _playerPrefab;

    //�v���C���[�̃C���X�^���X
    private PlayerCore _playerCore; 

    async void Start()
    {
        // CancellationTokenSource�𐶐�
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
        //Title�V�[����ǂݍ���
        await SceneManager.LoadSceneAsync("Title");
        Debug.Log("Title�V�[�������[�h���ꂽ");
        //Title�V�[���̏�����
        await InitializeTitle();
        //�Q�[���X�^�[�g�{�^�����������܂ő҂�
        await _startButton.OnClickAsync(token);
        
    }

    public async UniTask Game(CancellationToken token)
    {
        //Game�V�[����ǂݍ���
        await SceneManager.LoadSceneAsync("Game");
        Debug.Log("Game�V�[�������[�h���ꂽ");
        //Game�V�[���̏�����
        await InitializeGame();
        //Player�����S����܂ő҂�
        await UniTask.WaitUntil(() => _playerCore.HP <= 0);
        //���U���g�\��

        //�Q�[�����I���܂ő҂�
    }

    public async UniTask InitializeTitle()
    {
        //�X�^�[�g�{�^���̊���
        Debug.Log("Title�V�[��������");
        _startButton = GameObject.FindGameObjectWithTag("StartButton").GetComponent<Button>();
        await UniTask.WaitUntil(() => _startButton != null);
    }

    public async UniTask InitializeGame()
    {
        Debug.Log("Game�V�[��������");
        //�v���C���[�̐���
        var player = Instantiate(_playerPrefab);
        //�v���C���[�̃C���X�^���X���L���b�V��
        _playerCore = player.GetComponent<PlayerCore>();
        //�v���C���[�̏������҂�
        await _playerCore.InitializedAsync;
    }
}

