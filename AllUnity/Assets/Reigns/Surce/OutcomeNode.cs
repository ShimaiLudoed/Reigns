using UnityEngine;
using XNode;

[NodeTint("#6F3C5A")]
[CreateNodeMenu("Reigns/Outcome Node")]
public class OutcomeNode : BaseNode
{
  [TextArea(2, 3)]
  public string outcomeText = "Something happened...";
    
  public int goldChange;
  public int peopleChange;
  public int armyChange;
  public int churchChange;
    
  [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
  public BaseNode enter;
    
  [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
  public BaseNode exit;
    
  public override void Execute(GameManager manager)
  {
    manager.ModifyResources(goldChange, peopleChange, armyChange, churchChange);
    manager.ShowOutcomeMessage(outcomeText);
  }
    
  public override BaseNode GetNextNode()
  {
    NodePort port = GetOutputPort("exit");
    if (port != null && port.Connection != null)
    {
      return port.Connection.node as BaseNode;
    }
    return null;
  }
}