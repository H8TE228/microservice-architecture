namespace CoreLib.TraceLogic.Interfaces
{
    public interface ITraceReader
    {
        string Name { get; }
        string GetValue();
    }
}