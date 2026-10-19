/* ==============================================================================
 * CLASS: SoundManagerSOEditor
 * DESCRIPTION: Custom Inspector for SoundManagerSO. Redirects to the UI Toolkit
 * Signal Dashboard for clean, centralized management.
 * ==============================================================================*/
using UnityEditor;
using UnityEngine;
using Resenha.Signal.Data;
using Resenha.Signal.Editor.UI;

namespace Resenha.Signal.Editor
{
    [CustomEditor(typeof(SoundManagerSO))]
    public class SoundManagerSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space(12);
            
            EditorGUILayout.HelpBox(
                "Signal utilizes a high-performance UI Toolkit Dashboard. Use the dashboard to manage audio libraries, import rules, and mixer settings safely.", 
                MessageType.Info);
            
            EditorGUILayout.Space(12);
            
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.12f, 0.44f, 0.55f); 
            
            if (GUILayout.Button("Open Signal Dashboard", GUILayout.Height(40)))
            {
                SignalAudioDashboard.ShowWindow();
            }
            
            GUI.backgroundColor = originalColor;
            
            EditorGUILayout.Space(10);
        }
    }
}