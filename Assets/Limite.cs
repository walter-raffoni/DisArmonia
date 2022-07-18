using UnityEngine;

public class Limite : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null) other.gameObject.GetComponent<Player>().currentHP = 0;
    }
}