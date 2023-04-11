using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, IDamage
{
    [SerializeField] int towerHP;
    public void TakeDamage(int amount)
    {
        towerHP -= 1;
        if (towerHP <= 0)
        {
            GameManager.Instance.TowerDestroyed();
        }
    }
}
