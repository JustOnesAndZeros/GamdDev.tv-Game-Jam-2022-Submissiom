﻿using System.Collections.Generic;

public class CloneController : Player
{
    public Queue<Action> Actions;

    private bool _isActive = true;

    protected override void Update()
    {
        PlayRecording();
        
        base.Update();
    }

    private void OnEnable()
    {
        Actions = new Queue<Action>();
        TimePassed = 0;
    }

    private void PlayRecording()
    {
        if (_isActive && Actions.TryPeek(out Action act))
        {
            if (TimePassed >= act.Time)
            {
                switch (act.ActionType)
                {
                    case 0:
                        Move(act.Value);
                        break;
                    case 1:
                        Jump(act.Value);
                        break;
                    default:
                        OnDeath();
                        break;
                }

                Actions.Dequeue();
            }
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        
        Move(0);
        _isActive = false;
        Animator.SetBool(AnimIsActive, _isActive);
    }
}