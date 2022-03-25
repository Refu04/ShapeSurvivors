using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyParam", menuName = "ScriptableObjects/CreateEnemyParam")]
public class EnemyParam : ScriptableObject
{
    public int HP;
    public float speed;
}
