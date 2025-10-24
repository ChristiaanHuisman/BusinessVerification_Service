﻿using BusinessVerification_Service.Api.Interfaces.HelpersInterfaces;
using FuzzySharp;

namespace BusinessVerification_Service.Api.Helpers
{
    public class DomainNameHelper : IDomainNameHelper
    {
        // Generic method
        //
        // Return fuzzy match score from 0 to 100
        public int FuzzyMatchScore(string variable1, string variable2)
        {
            // Various algorithms are available, WeightedRatio is
            // a good balance
            return Fuzz.WeightedRatio(variable1, variable2);
        }
    }
}
