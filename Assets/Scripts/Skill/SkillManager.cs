using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SkillManager : MonoBehaviour
{
    //�X�L���A�C�R��
    [SerializeField]
    private Sprite[] _icons;
    //�X�L���{�[�h
    [SerializeField]
    private SkillBoard _skillBoard;
    //�X�L���{�[�h�ɒ񎦂���X�L��
    private Skill[] _presentSkill;
    //�X�L��
    private Skill[] _skills;

    private void Start()
    {
        //�X�L�������E�o�^
        _skills = new Skill[_icons.Length];
        _skills[0] = new Spin(0,  _icons[0], GameManager.Instance.PlayerCore);
        //�X�L���{�[�h�ɒ񎦂���X�L��������
        _presentSkill = new Skill[3];
        //���x���A�b�v���ɃX�L���{�[�h��\������
        GameManager.Instance.PlayerCore.Level
            .Skip(1)
            .Subscribe(_ => {
                //�񎦂���X�L���I��
                for(int i = 0; i < _presentSkill.Length; i++)
                {
                    _presentSkill[i] = _skills[Random.Range(0, _skills.Length - 1)];
                }
                //�X�L���{�[�h�ɃX�L�����Z�b�g����
                _skillBoard.SetSkillBoard(_presentSkill);

            })
            .AddTo(this);
        //�X�L���{�[�h��̃X�L�����I�����ꂽ�ۂ�SetSkill�����s����
        //�X�L���{�[�h���\���ɂ���
        _skillBoard.SelectButtonClick1()
            .Subscribe(_ => {
                SetSkill(_presentSkill[0].Id);
                _skillBoard.InactiveBoard();
            });
        _skillBoard.SelectButtonClick2()
            .Subscribe(_ => {
                SetSkill(_presentSkill[1].Id);
                _skillBoard.InactiveBoard();
            });
        _skillBoard.SelectButtonClick3()
            .Subscribe(_ => {
                SetSkill(_presentSkill[2].Id);
                _skillBoard.InactiveBoard();
            });
    }

    public void SetSkill(int id)
    {
        //���ɏK���ς݂̃X�L���ł���΃��x���A�b�v
        if(GameManager.Instance.PlayerCore.FirstSkill.Id == id)
        {
            GameManager.Instance.PlayerCore.FirstSkill.LevelUp();
            return;
        }
        if (GameManager.Instance.PlayerCore.SecondSkill.Id == id)
        {
            GameManager.Instance.PlayerCore.SecondSkill.LevelUp();
            return;
        }
        if (GameManager.Instance.PlayerCore.ThirdSkill.Id == id)
        {
            GameManager.Instance.PlayerCore.ThirdSkill.LevelUp();
            return;
        }
        //�X�L���X���b�g���]���Ă���ΏK��
        if(GameManager.Instance.PlayerCore.FirstSkill.Id == 99)
        {
            GameManager.Instance.PlayerCore.FirstSkill = _skills[id];
            return;
        }
        if (GameManager.Instance.PlayerCore.SecondSkill.Id == 99)
        {
            GameManager.Instance.PlayerCore.SecondSkill = _skills[id];
            return;
        }
        if (GameManager.Instance.PlayerCore.ThirdSkill.Id == 99)
        {
            GameManager.Instance.PlayerCore.ThirdSkill = _skills[id];
            return;
        }
    }
}
