using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] MeshRenderer tower;

    [Header("----- Stats -----")]
    [SerializeField] int towerHP;
    public void TakeDamage(int amount)
    {
        towerHP -= 1;
        if (towerHP <= 0)
        {
            Destroy(gameObject);
            GameManager.Instance.TowerDestroyed();
        } 
        else
        {
            StartCoroutine(FlashColor());
        }
    }

    IEnumerator FlashColor()
    {
        tower.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        tower.material.color = Color.white;
    }
}
