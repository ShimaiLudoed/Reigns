using UnityEngine;
using UnityEditor;
using XNodeEditor;

[CustomEditor(typeof(Base))]
public class GameGraphEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();
        
    Base graph = (Base)target;
        
    EditorGUILayout.Space(10);
    EditorGUILayout.LabelField("Graph Tools", EditorStyles.boldLabel);
        
    if (GUILayout.Button("Open Graph Editor"))
    {
      NodeEditorWindow.Open(graph);
    }
        
    if (GUILayout.Button("Create Start Decision Node"))
    {
      DecisionNode startNode = graph.AddNode<DecisionNode>();
      startNode.question = "START";
      startNode.position = new Vector2(100, 100);
      EditorUtility.SetDirty(graph);
      AssetDatabase.SaveAssets();
    }
        
    if (GUILayout.Button("List All Nodes"))
    {
      Debug.Log($"Graph has {graph.nodes.Count} nodes:");
      foreach (var node in graph.nodes)
      {
        Debug.Log($"- {node.name} ({node.GetType().Name})");
      }
    }
  }
}