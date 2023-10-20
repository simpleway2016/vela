namespace VelaAgent.Infrastructures
{
    public interface IInfoOutput
    {
        int Cols { get; }
        int Rows { get; }
        Task Output(string text);
        Task Output(byte[] data, int? length = null);
    }
}
