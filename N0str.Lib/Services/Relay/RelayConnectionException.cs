namespace N0str.Services.Relay
{
    [Serializable]
    public class RelayConnectionException : Exception
    {
        public RelayConnectionException()
        {
        }

        public RelayConnectionException(string? message) : base(message)
        {
        }

        public RelayConnectionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}