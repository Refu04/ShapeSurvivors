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
        //�Q�[���X�^�[�g�{�^�����������܂ő҂�
    }

    public async UniTask Game()
    {
        //�Q�[�����I���܂ő҂�
    }
}

