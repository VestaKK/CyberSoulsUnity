using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerState
{
    public AttackState(StateManager stateManager)
        : base(stateManager) { }

    public override void Enter()
    {
        if (Player.PlayerMelee.OnClick())
        {
            if (Player.LockOnTarget == null)
            {
                Player.LookAtMouse();
            }
            else
            {
                Player.LookAtTarget();
            }
        }
    }

    public override void Exit()
    {

    }

    public override void Update()
    {
        if (InputManager.GetKeyDown(InputAction.Attack)) 
        {
            if (Player.PlayerMelee.OnClick()) 
            {
                if (Player.LockOnTarget == null)
                {
                    Player.LookAtMouse();
                }
                else
                {
                    Player.LookAtTarget();
                }
            }
        }

        Player.Motion.GravityOnly();
    }

    public override void CheckSwitchStates()
    {
        if (Player.PlayerMelee.IsResting && !Player.PlayerMelee.IsAttacking) 
        {
            if (InputManager.GetKeyDown(InputAction.Roll))
            {
                _stateManager.SwitchState(_stateManager.Roll());
            }
            else if (Player.IsMoving()) 
            {
                _stateManager.SwitchState(_stateManager.Walk());
            }
            else
            {
                _stateManager.SwitchState(_stateManager.Idle());
            }
        }
    }
}
