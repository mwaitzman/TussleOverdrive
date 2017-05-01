﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAction {

    public string sprite_name;
    public int sprite_rate = 1;
    public bool loop = false;
    public int length = 1;

    public int current_frame;

    public ActionGroup set_up_actions = new ActionGroup();
    public ActionGroup actions_before_frame = new ActionGroup();
    public Dictionary<int,ActionGroup> actions_at_frame = new Dictionary<int,ActionGroup>();
    public ActionGroup actions_after_frame = new ActionGroup();
    public ActionGroup actions_at_last_frame = new ActionGroup();
    public ActionGroup actions_on_clank = new ActionGroup();
    public ActionGroup actions_on_prevail = new ActionGroup();
    public ActionGroup state_transition_actions = new ActionGroup();
    public ActionGroup tear_down_actions = new ActionGroup();

    public Dictionary<string, Hitbox> hitboxes = new Dictionary<string, Hitbox>();
    public Dictionary<string, HitboxLock> hitbox_locks = new Dictionary<string, HitboxLock>();

    protected int last_frame;
    protected BattleObject actor;
    protected BattleController game_controller;
    
    public List<bool> cond_list = new List<bool> { true };
    public int cond_depth = 0;

    public virtual void SetUp (BattleObject obj) {
        last_frame = length;
        actor = obj;
        actor.BroadcastMessage("ChangeSprite",sprite_name);
        game_controller = BattleController.current_battle;
        foreach (string subaction in set_up_actions.subactions)
        {
            if (cond_list[cond_depth])
                SubactionLoader.executeSubaction(subaction, actor, this);
        }
    }

    // Update is called once per frame
    public virtual void Update() {
        if (sprite_rate != 0) // if it's zero, no need to animate
        {
            int sprite_number = Mathf.FloorToInt(current_frame / sprite_rate);
            if (sprite_rate < 0)
                sprite_number = Mathf.FloorToInt(current_frame / sprite_rate) - 1;
            actor.GetSpriteHandler().ChangeSubimage(sprite_number, loop);
        }

        foreach (string subaction in actions_before_frame.subactions)
            if (cond_list[cond_depth])
                SubactionLoader.executeSubaction(subaction, actor, this);

        if (actions_at_frame.ContainsKey(current_frame))
            foreach (string subaction in actions_at_frame[current_frame].subactions)
                if (cond_list[cond_depth])
                    SubactionLoader.executeSubaction(subaction, actor, this);

        if (current_frame >= last_frame)
            this.OnLastFrame();
	}

    public virtual void OnLastFrame()
    {
        foreach (string subaction in actions_at_last_frame.subactions)
            if (cond_list[cond_depth])
                SubactionLoader.executeSubaction(subaction, actor, this);
    }

    public virtual void LateUpdate() //This way the frame gets incremented after everything else
    {
        foreach (string subaction in actions_after_frame.subactions)
            if (cond_list[cond_depth])
                SubactionLoader.executeSubaction(subaction, actor, this);
        current_frame++;
    }

    public virtual void TearDown(GameAction new_action)
    {
        //Deactivate and destroy hitboxes at the end of the action
        foreach (Hitbox hbox in hitboxes.Values)
        {
            hbox.Deactivate();
            GameObject.Destroy(hbox.gameObject);
        }
        foreach (string subaction in tear_down_actions.subactions)
            if (cond_list[cond_depth])
                SubactionLoader.executeSubaction(subaction, actor, this);
    }

    public virtual void stateTransitions()
    {
        foreach (string subaction in state_transition_actions.subactions)
            if (cond_list[cond_depth])
                SubactionLoader.executeSubaction(subaction, actor, this);
    }

    public virtual void onClank()
    {
        foreach (string subaction in actions_on_clank.subactions)
            if (cond_list[cond_depth])
                SubactionLoader.executeSubaction(subaction, actor, this);
    }

    public virtual void onPrevail()
    {
        foreach (string subaction in actions_on_prevail.subactions)
            if (cond_list[cond_depth])
                SubactionLoader.executeSubaction(subaction, actor, this);
    }

    public void SetDynamicAction(DynamicAction dynAction)
    {
        length = dynAction.length;
        sprite_name = dynAction.sprite;
        sprite_rate = dynAction.sprite_rate;
        loop = dynAction.loop;

        set_up_actions = dynAction.set_up_actions;
        actions_before_frame = dynAction.actions_before_frame;
        actions_at_frame = dynAction.actions_at_frame_dict;
        actions_after_frame = dynAction.actions_after_frame;
        actions_at_last_frame = dynAction.actions_at_last_frame;
        state_transition_actions = dynAction.state_transition_actions;
        tear_down_actions = dynAction.tear_down_actions;
    }
}
