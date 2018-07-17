﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelectionButtonRig : MonoBehaviour {
    public GameObject actionSelectionButtonPrefab;

    private List<GameObject> children = new List<GameObject>();
    private UIGrid grid;

	// Use this for initialization
	void Start () {
        grid = GetComponent<UIGrid>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnModelChanged()
    {
        //If the action file has changed, we need to reload all of our action buttons
        if (LegacyEditorData.instance.loadedActionFileDirty)
        {
            //Get rid of our old list
            foreach(GameObject child in children)
            {
                NGUITools.Destroy(child);
            }
            children.Clear(); //Empty the list for future use

            //Create all the new buttons
            foreach(DynamicAction action in LegacyEditorData.instance.loadedActionFile.actions)
            {
                instantiateButton(action);
            }

            //Realign the grid
            grid.Reposition();
        }
    }

    private void instantiateButton(DynamicAction action)
    {
        GameObject go = NGUITools.AddChild(gameObject, actionSelectionButtonPrefab);
        ActionSelectionButton button = go.GetComponent<ActionSelectionButton>();
        button.SetAction(action);
        children.Add(go);
    }
}