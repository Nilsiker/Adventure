namespace Shellguard.Traits;

using Chickensoft.GodotNodeInterfaces;

public interface IStateDebug : INode
{
  string NodeName { get; }
  string State { get; }
  void AddToDebugGroup() => AddToGroup("StateDebug");
}
