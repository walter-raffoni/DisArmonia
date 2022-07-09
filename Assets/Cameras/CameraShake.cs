using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraShake : MonoBehaviour
{
    public UnityEvent shake;
    public static CameraShake instance;

    private void Awake()
    {
        instance = this;
    }

    public void ShakeEvent()
    {
        shake.Invoke();
    }
}
