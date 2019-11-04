using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureMatchingMethods
{
    public class MatchedFeatureCandidateSet
    {

        #region Properties


        public List<MatchedFeature> MatchedFeatures { get; set; }


        public string Error { get; set; }

        [XmlIgnore]
        public Exception Exception { get; set; }
        public bool ErrorOccurred { get; set; }

        public bool IsExactMatch { get; set; }
        public bool IsSubstringMatch { get; set; }
        public bool IsSoundexMatch { get; set; }
        public bool IsRelaxedMatch { get; set; }
        public bool IsCompositeMatch { get; set; }

        #endregion

        public MatchedFeatureCandidateSet()
        {
            MatchedFeatures = new List<MatchedFeature>();
        }

        public void Add(MatchedFeature matchedFeature)
        {
            MatchedFeatures.Add(matchedFeature);
        }

        public bool ContainsFeatureAddress(MatchedFeature matchedFeature)
        {
            bool ret = false;
            {
                foreach (MatchedFeature existingMatchedFeature in MatchedFeatures)
                {
                    if (existingMatchedFeature.MatchedFeatureAddress.Equals(matchedFeature.MatchedFeatureAddress))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        public bool ContainsFeatureId(MatchedFeature matchedFeature)
        {
            bool ret = false;
            {
                foreach (MatchedFeature existingMatchedFeature in MatchedFeatures)
                {
                    if (String.Compare(existingMatchedFeature.PrimaryIdValue, matchedFeature.PrimaryIdValue, true) == 0)
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        public MatchedFeature GetTopCandidate()
        {
            MatchedFeature ret = null;
            try
            {
                if (MatchedFeatures.Count > 0)
                {
                    double maxScore = 0;
                    foreach (MatchedFeature matchedFeature in MatchedFeatures)
                    {
                        if (matchedFeature.MatchScore > maxScore)
                        {
                            maxScore = matchedFeature.MatchScore;
                            ret = matchedFeature;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Exception in GetTopCandidate: " + e.Message, e);
            }
            return ret;
        }

        public double GetTopCandidateScore()
        {
            double ret = 0;
            try
            {
                if (MatchedFeatures.Count > 0)
                {
                    foreach (MatchedFeature matchedFeature in MatchedFeatures)
                    {
                        if (matchedFeature.MatchScore > ret)
                        {
                            ret = matchedFeature.MatchScore;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Exception in GetTopCandidate: " + e.Message, e);
            }
            return ret;
        }

        public bool IsValid(double minimumScore)
        {

            bool ret = false;

            if (MatchedFeatures.Count > 0)
            {
                if (GetTopCandidateScore() >= minimumScore)
                {
                    ret = true;
                }
            }

            return ret;

        }

    }
}
