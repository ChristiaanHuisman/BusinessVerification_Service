using BusinessVerification_Service.Api.Interfaces.HelpersInterfaces;

namespace BusinessVerification_Service.Api.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        // Returns a string of HTML
        public string BuildVerificationEmailHtml(string businessName, string verificationLink)
        {
            // Escape HTML in case user-provided values contain unsafe characters
            string safeBusinessName = System.Net.WebUtility.HtmlEncode(businessName ?? "Business");

            // HTML string with variables
            return $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8' />
                    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                    <title>Verify Your Email</title>
                </head>
                <body style='font-family: Arial, sans-serif; background-color: #f4f4f7; padding: 20px;'>
                    <table role='presentation' width='100%' cellspacing='0' cellpadding='0'>
                        <tr>
                            <td align='center'>
                                <table style='max-width: 600px; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 6px rgba(0,0,0,0.1);'>
                                    <tr>
                                        <td style='background-color: #0059ff; color: #ffffff; text-align: center; padding: 20px 0;'>
                                            <h2 style='margin: 0;'>EngagePoint</h2>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 30px; color: #333333;'>
                                            <h3 style='margin-top: 0;'>Verify your email address</h3>
                                            <p>Hi there <strong>{safeBusinessName}</strong> &#128075;</p>
                                            <p>Thank you for using with EngagePoint. To verify your email address, please click the button below:</p>

                                            <p style='text-align: center; margin: 30px 0;'>
                                                <a href='{verificationLink}' 
                                                style='background-color: #0059ff; color: white; text-decoration: none; padding: 12px 24px; border-radius: 6px; display: inline-block;'>
                                                Verify Email
                                                </a>
                                            </p>

                                            <p>If you didn’t request this, you can safely ignore this email — your account won’t be affected.</p>

                                            <p style='margin-top: 30px; font-size: 12px; color: #888;'>This link will expire in 24 hours.</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='background-color: #f4f4f7; text-align: center; padding: 15px; font-size: 12px; color: #888;'>
                                            &copy; {DateTime.UtcNow.Year} EngagePoint • All rights reserved
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>"
            ;
        }
    }
}
