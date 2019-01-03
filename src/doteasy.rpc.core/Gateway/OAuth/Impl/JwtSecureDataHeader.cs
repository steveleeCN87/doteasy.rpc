namespace DotEasy.Rpc.Gateway.OAuth.Impl
{
    public class JwtSecureDataHeader
    {
        public JwtSecureDataType Type { get; set; }

        public EncryptMode EncryptMode { get; set; }

        public string TimeStamp { get; set; }
    }
}