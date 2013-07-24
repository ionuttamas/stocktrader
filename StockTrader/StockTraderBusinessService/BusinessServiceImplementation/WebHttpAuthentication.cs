//  .NET StockTrader Sample WCF Application for Benchmarking, Performance Analysis and Design Considerations for Service-Oriented Applications
//                   3/1/2012: Updated to version 6.0, with notable enhancements for Windows Azure hosting and mobile compatibility. See: 
//                                  1. Technical overview paper: https://azurestocktrader.blob.core.windows.net/docs/Trade6Overview.pdf
//                                  2. MSDN Site with downloads, additional information: http://msdn.microsoft.com/stocktrader
//                                  3. Discussion Forum: http://social.msdn.microsoft.com/Forums/en-US/dotnetstocktradersampleapplication
//                                  4. Live on Windows Azure: https://azurestocktrader.cloudapp.net
//                                   
//
//  Configuration Service 6 Notes:
//                      The application implements Configuration Service 6.0, for which the source code also ships in the sample. However, the .NET StockTrader 6
//                      sample is a general-purpose performance sample application for Windows Server and Windows Azure even if you are not implementing the Configuration Service. 
//                      
//

using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace Trade.BusinessServiceImplementation
{
    /// <summary>
    /// Custom service authentication manager that extracts credentials from Authorization header. It doesn't fail if credentials
    /// are missing or invalid (it just provides null credentials). Whether this causes a failure is determined by the operation.
    /// </summary>
    public class WebHttpAuthentication : ServiceAuthenticationManager
    {
        public override ReadOnlyCollection<IAuthorizationPolicy> Authenticate(ReadOnlyCollection<IAuthorizationPolicy> authPolicy, Uri listenUri, ref Message message)
        {
            // If we're running in a REST service, try and extract credentials
            if (WebOperationContext.Current != null)
            {
                string basicAuthorization = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.Authorization];
                NetworkCredential credentials = GetCredentials(basicAuthorization);
                if (credentials != null)
                {
                    // If credentials are successfully parsed out, then store them for later use
                    ApplyCredentials(message, credentials);
                }
            }

            return base.Authenticate(authPolicy, listenUri, ref message);
        }
        
        /// <summary>
        /// Store the new user in the message properties for later use. Repository domain users get the role superuser (i.e. can trade on behalf of others)
        /// and all other users get the role of user (i.e. can trade as themselves only).
        /// </summary>
        private static void ApplyCredentials(Message message, NetworkCredential credentials)
        {
            string role = null;
            if (string.Equals(credentials.Domain, "repository", StringComparison.OrdinalIgnoreCase))
            {
                if (IsRepositoryCredentialValid(credentials))
                {
                    role = "superuser";
                }
            }
            else
            {
                if (IsUserCredentialValid(credentials))
                {
                    role = "user";
                }
            }

            if (role != null)
            {
                IPrincipal genericPrincipal = new GenericPrincipal(new GenericIdentity(credentials.UserName), new[] { role });
                message.Properties["Principal"] = genericPrincipal;
            }
        }

        /// <summary>
        /// Validate a user that is defined in the repository (i.e. a system user)
        /// </summary>
        private static bool IsRepositoryCredentialValid(NetworkCredential credentials)
        {
            bool isValid = false;

            try
            {
                new CustomUserNameValidator().Validate(credentials.UserName, credentials.Password);
                isValid = true;
            }
            catch (Exception)
            {
            }

            return isValid;
        }

        /// <summary>
        /// Validate a user that is defined in the users table (i.e. an end-user)
        /// </summary>
        private static bool IsUserCredentialValid(NetworkCredential credentials)
        {
            return new TradeEngine().validate(credentials.UserName, credentials.Password);
        }

        /// <summary>
        /// Parse credentials from authorization header (assumes basic HTTP authentication)
        /// </summary>
        /// <param name="authorization">Authorization header</param>
        private static NetworkCredential GetCredentials(string authorization)
        {
            NetworkCredential credentials = null;
            if (!string.IsNullOrWhiteSpace(authorization))
            {
                string[] tokens = authorization.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Look for a string in the format 'Basic <base64string>'
                if (tokens.Length == 2 && string.Equals(tokens[0], "Basic", StringComparison.OrdinalIgnoreCase))
                {
                    string decodedValue = Encoding.ASCII.GetString(Convert.FromBase64String(tokens[1]));
                    string[] credentialsArray = decodedValue.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    if (credentialsArray.Length == 2)
                    {
                        // We support domains as part of the user name so check if the username is prefixed before creating the credential
                        string[] userArray = credentialsArray[0].Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                        if (userArray.Length <= 2)
                        {
                            credentials = new NetworkCredential(userArray.Length == 2 ? userArray[1] : userArray[0],
                                                                credentialsArray[1],
                                                                userArray.Length == 2 ? userArray[0] : null);
                        }
                    }
                }
            }

            return credentials;
        }
    }
}
