using UnityEngine;
using XNode;

[NodeTint("#3C5A6F")]
[CreateNodeMenu("Reigns/Decision Node")]
public class DecisionNode : BaseNode
{
  [TextArea(3, 5)]
  public string question = "What will you do?";
    
  public Sprite cardIcon;
    
  [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Multiple)]
  public BaseNode leftInput;
    
  [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Multiple)]
  public BaseNode rightInput;
    
  [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
  public BaseNode swipeLeft;
    
  [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
  public BaseNode swipeRight;
    
  public BaseNode GetNextNode(bool isLeftSwipe)
  {
    if (isLeftSwipe)
    {
      NodePort port = GetOutputPort("swipeLeft");
      if (port != null && port.Connection != null)
      {
        return port.Connection.node as BaseNode;
      }
    }
    else
    {
      NodePort port = GetOutputPort("swipeRight");
      if (port != null && port.Connection != null)
      {
        return port.Connection.node as BaseNode;
      }
    }
    return null;
  }
}