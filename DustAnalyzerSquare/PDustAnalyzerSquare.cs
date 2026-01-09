namespace DustAnalyzerSquare
{
    internal class PDustAnalyzerSquare
    {
        [CHProtocol(2)]
        public float k { get; set; }

        [CHProtocol(4)]
        public float b { get; set; }

        [CHProtocol(85)]
        public float 浓度 { get; set; }

        [CHProtocol(154)]
        public float 量程 { get; set; }

        [CHProtocol(156)]
        public ushort 不要 { get; set; }

        [CHProtocol(166)]
        public ushort 状态 { get; set; }
    }
}
