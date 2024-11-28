namespace Odin.Email.Office365;

public static class Utility
{
    public static Microsoft.Graph.Models.EmailAddress ToOffice365EmailAddress(this EmailAddress address)
    {
        return new Microsoft.Graph.Models.EmailAddress
        {
            Address = address.Address,
            Name = address.DisplayName
        };
    }
}