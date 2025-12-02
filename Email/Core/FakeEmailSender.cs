namespace Odin.Email
{
    /// <summary>
    /// Fake IEmailSender for unit testing..
    /// </summary>
    public sealed class FakeEmailSender : IEmailSender
    {
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<ResultValue<string?>> SendEmail(IEmailMessage email)
        {
            return await Task.FromResult(ResultValue<string?>.Succeed("12345"));
        }
    }
}
