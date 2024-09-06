namespace Odin.Email.Office365;

public record Office365Options
{
    public MicrosoftGraphClientSecretCredentials? MicrosoftGraphClientSecretCredentials { get; init; }
};