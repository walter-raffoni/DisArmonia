using UnityEngine;
using UnityEngine.InputSystem;

public class ExtendedInputActions : MonoBehaviour
{
    private InputActions actions;
    private InputAction move, jump, dash, escape, baseAttack, cambiaStance, reloadGame;

    private void Awake()
    {
        actions = new InputActions();

        //Player Keys
        move = actions.Player.Move;
        jump = actions.Player.Jump;
        dash = actions.Player.Dash;
        baseAttack = actions.Player.BaseAttack;
        cambiaStance = actions.Player.CambiaStance;

        //Game Keys
        escape = actions.Game.Pause;
        reloadGame = actions.Game.ReloadGame;//DEBUG
    }

    private void OnEnable() => actions.Enable();

    private void OnDisable() => actions.Disable();

    public FrameInput HandleInput()
    {
        return new FrameInput
        {
            //Player Check
            JumpDown = jump.WasPressedThisFrame(),
            JumpHeld = jump.IsPressed(),
            DashDown = dash.WasPressedThisFrame(),
            AttackDown = baseAttack.WasPressedThisFrame(),
            BaseAttackReleased = baseAttack.WasReleasedThisFrame(),
            CambiaStanceDown = cambiaStance.WasPressedThisFrame(),

            X = move.ReadValue<Vector2>().x,
            Y = move.ReadValue<Vector2>().y,

            //Game Check
            PauseDown = escape.WasPressedThisFrame(),
            ReloadGameDown = reloadGame.WasPressedThisFrame()
        };
    }
}