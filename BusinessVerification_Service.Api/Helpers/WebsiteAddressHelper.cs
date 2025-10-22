namespace BusinessVerification_Service.Api.Helpers
{
    public class WebsiteAddressHelper
    {
        // Return true if the website address has
        // a supported scheme, only has one scheme,
        // or does not have a scheme at all
        public bool VerifyWebsiteAddressScheme(string website)
        {
            // Check is a scheme exists in the website address
            if (website.Contains("://"))
            {
                // Count number of schemes in website address
                // There should not be more that 1 scheme
                int schemeCount = website.Split("://").Length - 1;
                if (schemeCount > 1)
                {
                    return false;
                }

                // Get first part of website address before ://
                // Only http, https and ftp are supported schemes
                string scheme = website.Split("://")[0];
                if (scheme != "http" && scheme != "https" && scheme != "ftp")
                {
                    return false;
                }
            }

            return true;
        }

        // Return a correctly built URI formatted website address
        public string BuildUriWebsiteAddress(string website)
        {
            // Add https scheme if no scheme exists
            UriBuilder uriBuilder = new UriBuilder(
                website.StartsWith("http", StringComparison.Ordinal)
                || website.StartsWith("ftp", StringComparison.Ordinal)
                ? website
                : $"https://{website}"
            );

            return uriBuilder.Uri.ToString();
        }
    }
}
