using UnityEngine;

public interface IVita<T>
{
    void TakeDamage(T damageTaken);
}

public interface IPlayerEffector
{
    public Vector2 EvaluateEffector();
}

public struct FrameInput
{
    //Player
    public float X, Y;
    public bool JumpDown;
    public bool JumpHeld;
    public bool DashDown;
    public bool AttackDown;
    public bool BaseAttackReleased;
    public bool BrutalAttackDown;
    public bool BrutalAttackReleased;
    public bool CambiaStanceDown;

    //Game
    public bool PauseDown;
    public bool ReloadGameDown;
}