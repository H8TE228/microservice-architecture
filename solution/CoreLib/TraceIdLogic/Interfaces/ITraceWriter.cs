namespace CoreLib.TraceLogic.Interfaces
{
    public interface ITraceWriter
    {
        string Name { get; }
        void WriteValue(string value);
    }
}