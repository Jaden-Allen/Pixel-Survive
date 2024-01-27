using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
