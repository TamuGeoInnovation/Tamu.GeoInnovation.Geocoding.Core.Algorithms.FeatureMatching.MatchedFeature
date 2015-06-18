using System;
using System.Xml.Serialization;
using USC.GISResearchLab.Common.Addresses;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureMatchingMethods;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureMatchScorers.MatchScoreResults;
using USC.GISResearchLab.Geocoding.Core.Metadata.ReferenceSources;
using USC.GISResearchLab.Geocoding.Core.OutputData.Error;
using USC.GISResearchLab.Geocoding.Core.ReferenceDatasets.ReferenceFeatures;
using USC.GISResearchLab.Geocoding.Core.ReferenceDatasets.ReferenceFeatures.Implementations;

namespace USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching
{
    public class MatchedFeature
    {
        #region Properties
        public StreetAddress QueryAddress { get; set; }
        public StreetAddress ParsedAddress { get; set; }
        public RelaxableStreetAddress MatchedAddress { get; set; }
        public StreetAddress MatchedFeatureAddress { get; set; }
        public FeatureMatchTypes FeatureMatchTypes { get; set; }
        public ReferenceSourceType ReferenceSourceType { get; set; }
        public bool IsExactMatch { get; set; }
        public bool IsSubstringMatch { get; set; }
        public bool IsSoundexMatch { get; set; }
        public bool IsRelaxedMatch { get; set; }
        public bool IsCompositeMatch { get; set; }
        //public Geometry MatchedGeometry { get; set; }
        public IReferenceFeature MatchedReferenceFeature { get; set; }
        public bool Valid { get; set; }

        public string Error { get; set; }
        public bool ExceptionOccurred { get; set; }

        [XmlIgnore]
        public Exception Exception { get; set; }

        public string PrimaryIdField { get; set; }
        public string PrimaryIdValue { get; set; }
        public string SecondaryIdField { get; set; }
        public string SecondaryIdValue { get; set; }

        public double MatchScore { get; set; }
        public MatchScoreResult MatchScoreResult { get; set; }

        public DateTime MatchScoreStartTimer { get; set; }
        public DateTime MatchScoreEndTimer { get; set; }

        public GeocodedError GeocodedError { get; set; }

        public double MatchScoreTimeTaken
        {
            get
            {
                long diff = MatchScoreEndTimer.Ticks - MatchScoreStartTimer.Ticks;
                TimeSpan t = TimeSpan.FromTicks(diff) ;
                return t.TotalMilliseconds;
            }
        }

        public DateTime MatchTypeStartTimer { get; set; }
        public DateTime MatchTypeEndTimer { get; set; }

        public double MatchTypeTimeTaken
        {
            get
            {
                long diff = MatchTypeEndTimer.Ticks - MatchTypeStartTimer.Ticks;
                TimeSpan t = TimeSpan.FromTicks(diff);
                return t.TotalMilliseconds;
            }
        }

        #endregion

        public MatchedFeature()
        {
            ParsedAddress = new StreetAddress();
            MatchedAddress = new RelaxableStreetAddress();
            MatchedFeatureAddress = new StreetAddress();
            QueryAddress = new StreetAddress();

            GeocodedError = new GeocodedError();

            // use the nickle feature as the default
            MatchedReferenceFeature = new NickleReferenceFeature();

            PrimaryIdField = "";
            PrimaryIdValue = "";

            SecondaryIdField = "";
            SecondaryIdValue = "";
        }

        public override string ToString()
        {
            string ret = "";
            if (MatchedReferenceFeature != null)
            {
                ret = MatchedReferenceFeature.StreetAddressableGeographicFeature.ToString();
            }
            return ret;
        }
    }
}
