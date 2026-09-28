namespace SchoolApp.Models.DataModels.SecurityModels
{
    public class RefreshTokenRequest
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
