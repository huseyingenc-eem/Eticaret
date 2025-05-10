namespace ETicaret.Application.Services.JwtServices;

public class CustomTokenOptions
{
    public string[] Audience { get; set; } 
    public string Issuer { get; set; }     
    public int AccessTokenExpiration { get; set; } 
    public string SecurityKey { get; set; } 

}
