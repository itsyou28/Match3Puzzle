using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FiniteStateMachine;

[CustomEditor(typeof(ReactionByState))]
public class CustomReactionByState : Editor
{
    SerializedProperty arrSwitch;

    SerializedProperty isDebug;
    SerializedProperty hideFirstFrame;

    private void OnEnable()
    {
        FSM_Utility.Initialize();
        
        arrSwitch = serializedObject.FindProperty("arrSwitch");

        isDebug = serializedObject.FindProperty("isDebug");
        hideFirstFrame = serializedObject.FindProperty("hideFirstFrame");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        SetReaction(arrSwitch, "Reaction Number");

        isDebug.boolValue = EditorGUILayout.Toggle("isDebug", isDebug.boolValue);
        hideFirstFrame.boolValue = EditorGUILayout.Toggle("hideFirstFrame", hideFirstFrame.boolValue);

        serializedObject.ApplyModifiedProperties();
    }
    
    private void SetReaction(SerializedProperty targetSP, string label)
    {
        targetSP.arraySize = EditorGUILayout.IntField(label, targetSP.arraySize);

        for (int idx = 0; idx < targetSP.arraySize; idx++)
        {
            SerializedProperty sp = targetSP.GetArrayElementAtIndex(idx);
            SerializedProperty layer = sp.FindPropertyRelative("layer");
            SerializedProperty fsm = sp.FindPropertyRelative("fsm");
            SerializedProperty state = sp.FindPropertyRelative("state");

            EditorGUILayout.BeginHorizontal();
            layer.enumValueIndex = (int)(FSM_LAYER_ID)EditorGUILayout.EnumPopup((FSM_LAYER_ID)layer.enumValueIndex);
            fsm.enumValueIndex = (int)(FSM_ID)EditorGUILayout.EnumPopup((FSM_ID)fsm.enumValueIndex);

            StatePopupList p = FSM_Utility.GetPopupList((FSM_ID)fsm.enumValueIndex);
            state.intValue = EditorGUILayout.IntPopup(state.intValue, p.arr_sName, p.arr_nID);
            EditorGUILayout.EndHorizontal();
            
            SetReactionRow(ref sp, "startBefore", "onStartBefore", "onStartBeforeExcuteId");
            SetReactionRow(ref sp, "Start", "onStart", "onStartExcuteId");
            SetReactionRow(ref sp, "StartAfter1", "onStartAfter1", "onStartAfter1ExcuteId");
            SetReactionRow(ref sp, "StartAfter2", "onStartAfter2", "onStartAfter2ExcuteId");
            SetReactionRow(ref sp, "EndBefore", "onEndBefore", "onEndBeforeExcuteId");
            SetReactionRow(ref sp, "End", "onEnd", "onEndExcuteId");
            SetReactionRow(ref sp, "EndAfter", "onEndAfter", "onEndAfterExcuteId");
            SetReactionRow(ref sp, "Pause", "onPause", "onPauseExcuteId");
            SetReactionRow(ref sp, "Resume", "onResume", "onResumeExcuteId");
            
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        }
    }

    void SetReactionRow(ref SerializedProperty target, string label, string targetState, string targetExcuteID)
    {
        SerializedProperty sp_reactionID = target.FindPropertyRelative(targetState);
        SerializedProperty sp_excuteid = target.FindPropertyRelative(targetExcuteID);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(100));
        sp_reactionID.enumValueIndex = (int)(ReactionID)EditorGUILayout.EnumPopup((ReactionID)sp_reactionID.enumValueIndex, GUILayout.Width(100));

        if (sp_reactionID.enumValueIndex == (int)ReactionID.Excute)
            sp_excuteid.intValue = EditorGUILayout.IntField(sp_excuteid.intValue, GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();
    }

}
