using UnityEngine;

public class Room : MonoBehaviour
{
    private int totalEnemies, totalAgile, totalBrutale;

    public int TotalEnemies => totalEnemies;
    public int TotalAgile
    {
        get { return totalAgile; }
        set { totalAgile = value; }
    }
    public int TotalBrutale
    {
        get { return totalBrutale; }
        set { totalBrutale = value; }
    }

    private void Start()
    {
        if (GetComponentsInChildren<Enemy>() != null) totalEnemies++;
    }
}
