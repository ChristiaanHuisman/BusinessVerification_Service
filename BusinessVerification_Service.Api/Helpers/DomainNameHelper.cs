using FuzzySharp;

namespace BusinessVerification_Service.Api.Helpers
{
    public class DomainNameHelper
    {
        // Return true if registerable domains are an exact match
        public bool DomainExactMatch(string emailRegisterableDomain,
            string websiteRegisterableDomain)
        {
            return emailRegisterableDomain == websiteRegisterableDomain;
        }

        // Return fuzzy match score from 0 to 100
        public int FuzzyMatchScore(string domain, string businessName)
        {
            // Various algorithms are available, WeightedRatio is
            // a good balance
            return Fuzz.WeightedRatio(domain, businessName);
        }
    }
}
