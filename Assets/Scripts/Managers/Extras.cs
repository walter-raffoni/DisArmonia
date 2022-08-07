using UnityEngine;

public struct FrameInput
{
    //Player
    public float X, Y;
    public bool JumpDown;
    public bool JumpHeld;
    public bool DashDown;
    public bool AttackDown;
    public bool BrutalAttackDown;
    public bool CambiaStanceDown;

    //Game
    public bool PauseDown;
    public bool ReloadGameDown;
}