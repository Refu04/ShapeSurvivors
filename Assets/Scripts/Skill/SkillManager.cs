using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SkillManager : MonoBehaviour
{
    //スキルアイコン
    [SerializeField]
    private Sprite[] _icons;
    //スキルボード
    [SerializeField]
    private SkillBoard _skillBoard;
    //スキルボードに提示するスキル
    private Skill[] _presentSkill;
    //スキル
    private Skill[] _skills;

    private void Start()
    {
        //スキル生成・登録
        _skills = new Skill[_icons.Length];
        _skills[0] = new Spin(0,  _icons[0], GameManager.Instance.PlayerCore);
        //スキルボードに提示するスキル初期化
        _presentSkill = new Skill[3];
        //レベルアップ時にスキルボードを表示する
        GameManager.Instance.PlayerCore.Level
            .Skip(1)
            .Subscribe(_ => {
                //提示するスキル選択
                for(int i = 0; i < _presentSkill.Length; i++)
                {
                    _presentSkill[i] = _skills[Random.Range(0, _skills.Length - 1)];
                }
                //スキルボードにスキルをセットする
                _skillBoard.SetSkillBoard(_presentSkill);

            })
            .AddTo(this);
        //スキルボード上のスキルが選択された際にSetSkillを実行する
        //スキルボードを非表示にする
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
        //既に習得済みのスキルであればレベルアップ
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
        //スキルスロットが余っていれば習得
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
