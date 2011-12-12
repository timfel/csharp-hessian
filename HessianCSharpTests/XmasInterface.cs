namespace org.meet4xmas.wire
{
    public interface IServiceAPI
    {
        Response registerAccount(String userId);
    }

    public class Response
    {
        public bool success;
        public ErrorInfo error;
        public object payload;
    }

    public class ErrorInfo
    {
        public int code;
        public string message;
    }
}
