namespace AccountsProyect.BE
{
    public class Response
    {
        public int Code;
        public string? Message;
        public Object Data;

      public Response(int code, string? message, object data)
      {
            Code = code;
            Message = message;
            Data = data;
      }

      public Response(int code, string? message)
      {
            Code = code;
            Message = message;  
      }
    }
}