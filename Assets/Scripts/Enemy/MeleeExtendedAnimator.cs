using UnityEngine;

public class MeleeExtendedAnimator : MonoBehaviour
{
    public EnemyMelee melee;

    private void Start() => melee = GetComponentInParent<EnemyMelee>();

    public void DealDamage() => melee.DealDamage();//serve perch� unity � stronzo con l'animator messo in un oggetto figlio e non mostra le funzioni del padre
}
