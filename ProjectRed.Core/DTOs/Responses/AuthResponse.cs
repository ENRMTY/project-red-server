namespace ProjectRed.Core.DTOs.Responses
{
    public class AuthResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public bool RequiresProfileCompletion { get; set; }
        public T? Details { get; set; }
    }
}
