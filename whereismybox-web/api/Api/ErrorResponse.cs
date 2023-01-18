namespace Api;

public class ErrorResponse
{
    public string Message { get; set; }
    public string Details { get; set; }

    public ErrorResponse(string message, string details)
    {
        Message = message;
        Details = details;
    }
}