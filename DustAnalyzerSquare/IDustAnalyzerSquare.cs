using ProtocolInterface;

namespace DustAnalyzerSquare
{
    public interface IDustAnalyzerSquare : IProtocol
    {
        Task<Dictionary<string, string>?> Read(string addr, int tryCount = 0, CancellationToken cancelToken = default);
    }
}
