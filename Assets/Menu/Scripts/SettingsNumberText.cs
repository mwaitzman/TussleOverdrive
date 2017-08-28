﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsNumberText : MonoBehaviour {
    public string var_name;
    public string display_text;
    public int value;
    public string after_text;

    public int min_value;
    public int max_value;
    public int increment;

    public bool percentage = false;
    private UILabel label;

    void Start()
    {
        label = GetComponentInChildren<UILabel>();
        if (percentage)
            value = (int)((float)Settings.current_settings.GetSetting(var_name) * 100);
        else
            value = (int)Settings.current_settings.GetSetting(var_name);
        RedrawText();

    }

    void RedrawText()
    {
        label.text = display_text + value.ToString() + after_text;
        Settings.current_settings.ChangeSetting(var_name, value);
    }

    void IncrementValue()
    {
        if (value < max_value)
            value += increment;
        else
            value = min_value;
        RedrawText();
    }

    void DecrementValue()
    {
        if (value > min_value)
            value -= increment;
        else
            value = max_value;
        RedrawText();
    }
}
