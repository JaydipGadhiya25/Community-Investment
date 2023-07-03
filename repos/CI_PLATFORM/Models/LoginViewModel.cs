namespace CI_PLATFORM.Models
{
    /// <summary>
    /// Class for Login View
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets password
        /// </summary>
        public string Password { get; set; }

        public long? CountryId { get; set; }

        /// <summary>
        /// Gets or sets return url.
        /// </summary>
        public string? ReturnUrl { get; set; }
    }
}
