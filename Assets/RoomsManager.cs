using System.Collections.Generic;
using UnityEngine;

public class RoomsManager : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsList = new List<GameObject>();
    [SerializeField] List<GameObject> virtualCams = new List<GameObject>();
    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    public static RoomsManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeLevel(int id)
    {
        virtualCams[id - 1].SetActive(false);
        virtualCams[id].SetActive(true);

        objectsList[id - 1].SetActive(false);
        objectsList[id].SetActive(true);
    }
}