namespace Reineur.Share
{

    public class ActionResponse
    {

        public ActionResponse()
        {
            IsSuccess = true;
        }
        private ActionResponse(string errorMessage, ResponseType errorType = ResponseType.BadRequest)
        {
            ErrorType = errorType;
            ReasonPhrase = errorMessage;
        }

        private ActionResponse(Exception ex)
        {
            ReasonPhrase = $"Internal Serval Error: \n \t{ex.Message} ";
            ErrorType = ResponseType.ServerError;
            ServerException = ex;
        }
        public static ActionResponse SuccessResult() => new();

        public static ActionResponse NotFoundResult(string msg = "Record is not found.") => new(msg, ResponseType.NotFound);

        public static ActionResponse BadRequestResult(string msg = "Bad request.") => new(msg, ResponseType.BadRequest);

        public static ActionResponse ServerExceptionError(Exception ex) => new(ex);

        public static ActionResponse Unauthorized(string msg = "Unauthorize") => new(msg, ResponseType.Unauthorized);



        public ResponseType? ErrorType { get; private set; }
        public Exception? ServerException { get; private set; }
        public string ReasonPhrase { get; private set; } = string.Empty;
        public bool IsSuccess { get; private set; }
        public object? PayLoad { get; set; }

        public object? Information { get; set; }
    }
    public enum ResponseType
    {
        BadRequest = 1,
        NotFound = 2,
        ServerError = 3,
        Unauthorized = 4,
    }
}

