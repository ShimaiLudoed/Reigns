using UnityEngine;
using XNode;

[NodeTint("#4A4A4A")]
public class BaseNode : Node
{
  [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
  public BaseNode output;
    
  public virtual void Execute(GameManager manager)
  {
  }
    
  public virtual BaseNode GetNextNode()
  {
    NodePort port = GetOutputPort("output");
    if (port != null && port.Connection != null)
    {
      return port.Connection.node as BaseNode;
    }
    return null;
  }
    
  public override object GetValue(NodePort port)
  {
    return null;
  }
}