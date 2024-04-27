
namespace Process_DIAPI_SERVICE_LAYER.Models
{

    public class ErrorSL
    {
        public Error error { get; set; }
        
    }

    public class Error
    {
        public int code { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public string lang { get; set; }
        public string value { get; set; }
    }

}
