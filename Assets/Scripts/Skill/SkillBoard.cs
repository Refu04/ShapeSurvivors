using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class SkillBoard : MonoBehaviour
{
    //�X�L���{�[�h�ɒ񎦂�����
    [SerializeField]
    private Image[] _icons;
    [SerializeField]
    private Text[] _names;
    [SerializeField]
    private Text[] _explanations;
    //�X�L���I���{�^��
    [SerializeField]
    private Button[] _selectButton;
    //�{�^���̃N���b�N�C�x���g�����J����
    public IObservable<Unit> SelectButtonClick1()
    {
        return _selectButton[0].OnClickAsObservable();
    }
    public IObservable<Unit> SelectButtonClick2()
    {
        return _selectButton[1].OnClickAsObservable();
    }
    public IObservable<Unit> SelectButtonClick3()
    {
        return _selectButton[2].OnClickAsObservable();
    }

    public void SetSkillBoard(Skill[] skills)
    {
        //�X�L���{�[�h�\��
        for(int i = 0; i <  transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        //�{�[�h�̒��g�����蓖�Ă�
        for (int i = 0; i < skills.Length; i++)
        {
            _icons[i].sprite = skills[i].Icon;
            _names[i].text = skills[i].Name;
            _explanations[i].text = skills[i].Explanation;
        }
    }

    public void InactiveBoard()
    {
        //�X�L���{�[�h��\��
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
