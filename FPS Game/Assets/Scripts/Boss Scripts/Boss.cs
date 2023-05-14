using UnityEngine;

public class Boss : MonoBehaviour
{
    public int attackDamage = 20;

    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;

    public void Attack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider[] colInfo = Physics.OverlapSphere(pos, attackRange, attackMask);
        foreach (Collider collider in colInfo)
        {
            collider.GetComponent<PlayerController>().TakeDamage(attackDamage);
        }
    }
}
