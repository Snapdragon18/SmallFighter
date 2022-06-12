using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwing : FighterState
{
    public Throwing(NewFighter fighter) : base(fighter)
    {
    }

    public override void OnStateEnter()
    {
        fighter.currentFrame = 1;
        fighter.currentHitboxes.Clear();
        fighter.currentHurtboxes.Clear();
        fighter.velocity = Vector3.zero;
        fighter.gameObject.layer = 2;

        if (fighter.currentThrow != null)
        {
            if (!fighter.beingThrown)
            {
                fighter.SetModelLayer(NewFighter.FrontLayer);
                fighter.animator.Play($"Base Layer.{fighter.currentThrow.animationName}", -1, 0f);
            }                
            else
            {
                fighter.SetModelLayer(NewFighter.BackLayer);
                fighter.animator.Play($"Base Layer.{fighter.currentThrow.animationName}Dmg", -1, 0f);
            }
        }
    }

    public override void Update(InputData currentInput)
    {
        if (fighter.currentThrow == null || (!fighter.beingThrown && fighter.currentFrame >= fighter.currentThrow.numberOfFramesThrowing) || (fighter.beingThrown && fighter.currentFrame >= fighter.currentThrow.numberOfFramesThrown))
        {
            if (fighter.beingThrown)
            {
                fighter.animator.Play("Rising", -1, 0f);
                fighter.SwitchState(new Knockdown(fighter));
            }
            else
            {
                fighter.SwitchState(new Walking(fighter));
            }
        }
        else
        {
            if (fighter.beingThrown)
            {
                if (fighter.currentFrame <= 2 && fighter.CanBreakThrow())
                {
                    fighter.BreakThrow.Invoke();
                    return;
                }

                if (fighter.currentFrame == fighter.currentThrow.tossFrame)
                {
                    int side = fighter.IsOnLeftSide ? -1 : 1;
                    Vector2 offset = new Vector2(fighter.currentThrow.opponentTargetOffset.x * side, fighter.currentThrow.opponentTargetOffset.y);
                    fighter.controller.Move(offset);

                    fighter.velocity = new Vector2(fighter.currentThrow.tossSpeed * side, 0f);
                }
            }

            fighter.currentFrame += 1;
        }
    }

    public override void OnStateExit()
    {
        fighter.SetModelLayer(fighter.IsOnLeftSide ? NewFighter.FrontLayer : NewFighter.BackLayer);
        fighter.currentThrow = null;
        fighter.beingThrown = false;
        fighter.gameObject.layer = 6;
    }
}