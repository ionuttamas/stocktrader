using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Principal;
using System.ServiceModel;

namespace Trade.BusinessServiceImplementation
{
    /// <summary>
    /// Translate principal role to a claim.
    /// </summary>
    public class WebHttpAuthorizationPolicy : IAuthorizationPolicy
    {
        private readonly string _id = Guid.NewGuid().ToString();

        public string Id
        {
            get { return _id; }
        }

        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            object property;
            if (OperationContext.Current.IncomingMessageProperties.TryGetValue("Principal", out property))
            {
                IPrincipal principal = (IPrincipal)property;
                evaluationContext.Properties["Principal"] = principal;
                evaluationContext.Properties["Identities"] = new List<IIdentity> { principal.Identity };

                // The claim is determined by the user's role
                if (principal.IsInRole("superuser"))
                {
                    // Grant '*' user access (i.e. allowed to trade on behalf of all users)
                    evaluationContext.AddClaimSet(this, new DefaultClaimSet(new Claim("urn:Claim:UserAccess", "*", Rights.PossessProperty)));
                }
                else
                {
                    // Grant user access to specific user only (i.e. can only trade as the current user)
                    evaluationContext.AddClaimSet(this, new DefaultClaimSet(new Claim("urn:Claim:UserAccess", principal.Identity.Name, Rights.PossessProperty)));
                }
            }

            return true;
        }
    }
}
