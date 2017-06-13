﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : GameAction {
    private GameObject puff;

    public override void stateTransitions()
    {
        base.stateTransitions();
        if (current_frame >= last_frame)
        {
            actor.SetVar("landing_lag", 0);
            actor.BroadcastMessage("DoAction", "NeutralAction");
            //platform phase reset
            actor.BroadcastMessage("ChangeXPreferred", 0.0f);
        }
        StateTransitions.CheckGround(actor.GetAbstractFighter());
    }

    public override void Update()
    {
        base.Update();
        if (current_frame == 0)
        {
            puff = ObjectPooler.current_pooler.GetPooledObject("LandPuff", actor.transform);
            if (puff)
            {
                puff.SetActive(true);
                puff.transform.localPosition = new Vector3(0, -actor.transform.lossyScale.y);
            }
            
            actor.BroadcastMessage("ChangeYPreferred", actor.GetFloatVar("max_fall_speed"));
            last_frame = Mathf.Max(last_frame,actor.GetIntVar("landing_lag"));
            actor.BroadcastMessage("ChangeYSpeed", -1.0f);
            //L Cancel
            /* lcancel = settingsManager.getSetting('lagCancel')
            if lcancel == 'normal':
                if _actor.keyHeld('shield', 4) and not _actor.keyBuffered('shield', 20, 0.1, 4):
                    print("l-cancel")
                    self.last_frame = self.last_frame // 2
            elif lcancel == 'auto':
                print("l-cancel")
                self.last_frame = self.last_frame // 2 */
        }
    }

    public override void TearDown(GameAction new_action)
    {
        base.TearDown(new_action);
        puff.SendMessage("StopParticles");
    }
}
