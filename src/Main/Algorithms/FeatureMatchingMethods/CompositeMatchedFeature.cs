using Microsoft.SqlServer.Types;
using SQLSpatialTools;
using System;
using System.Collections.Generic;
using USC.GISResearchLab.Common.Addresses;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Common.Geometries;
using USC.GISResearchLab.Geocoding.Core.OutputData.Error;

namespace USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching
{

    public class CompositeMatchedFeature : MatchedFeature
    {
        #region Properties

        public bool ShouldNotContinue { get; set; }
        public MatchedFeature[] MatchedFeatures { get; set; }
        public RelaxableStreetAddress[] MatchedAddresses { get; set; }
        public StreetAddress[] MatchedFeatureAddresses { get; set; }



        #endregion

        public CompositeMatchedFeature()
            : base()
        {
            ParsedAddress = new StreetAddress();
        }

        public void BuildConvexHull()
        {
            try
            {
                List<String> addedIds = new List<string>();
                List<SqlGeography> geographies = new List<SqlGeography>();

                List<MatchedFeature> addedMatchedFeatures = new List<MatchedFeature>();

                for (int i = 0; i < MatchedFeatures.Length; i++)
                {
                    MatchedFeature matchedFeature = MatchedFeatures[i];

                    if (!addedIds.Contains(matchedFeature.PrimaryIdValue))
                    {
                        addedIds.Add(matchedFeature.PrimaryIdValue);
                        addedMatchedFeatures.Add(matchedFeature);
                        geographies.Add(matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeography);
                    }
                }


                //SqlGeography compositeGeography = Geometry.BuildSqlGeographyMultiPoint(geographies.ToArray(), 4269);
                //// convex hull will fail if points are collinear
                //try
                //{
                //    convexHullGeography = SQLSpatialToolsFunctions.ConvexHullGeography(compositeGeography);
                //    MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeography = convexHullGeography;
                //    convexHullGeographyArea = convexHullGeography.STArea().Value;
                //}
                //catch (Exception ex1) // if the convex hull fails, see if union makes a linestring and if so use the centroid
                //{
                //    SqlGeography unionGeography = null;
                //    for (int i=0; i< geographies.Count; i++)
                //    {
                //        if (i == 0)
                //        {
                //            unionGeography = geographies[i];
                //        }
                //        else
                //        {
                //            unionGeography = unionGeography.STUnion(geographies[i]);
                //        }
                //    }

                //    convexHullGeography = unionGeography;
                //}


                SqlGeography compositeGeography = null;
                for (int i = 0; i < geographies.Count; i++)
                {
                    if (i == 0)
                    {
                        compositeGeography = geographies[i].STBuffer(10);
                    }
                    else
                    {

                        SqlGeography currentGeography = geographies[i];
                        SqlGeography currentGeographyBufferred = currentGeography.STBuffer(10);
                        compositeGeography = Geometry.BuildSqlGeographyMultiPoint(new SqlGeography[] { compositeGeography, currentGeographyBufferred }, compositeGeography.STSrid.Value);
                    }
                }

                SqlGeometry compositeGeometry = SQLSpatialToolsFunctions.VacuousGeographyToGeometry(compositeGeography, compositeGeography.STSrid.Value);
                SqlGeometry convexHullGeometry = compositeGeometry.STConvexHull();
                SqlGeography convexHullGeography = SQLSpatialToolsFunctions.VacuousGeometryToGeography(convexHullGeometry, convexHullGeometry.STSrid.Value);

                MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry = convexHullGeometry;
                MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeography = convexHullGeography;

                GeocodedError.ErrorBounds = convexHullGeography.STArea().Value;
                GeocodedError.ErrorBoundsUnit = LinearUnitTypes.Meters;
                GeocodedError.ErrorCalculationType = GeocodedErrorCalculationType.featureConvexHullArea;

            }
            catch (Exception e)
            {
                throw new Exception("Error in BuildConvexHull: " + e.Message, e);
            }
        }



        public void AddMatchedFeature(MatchedFeature feature)
        {
            if (MatchedFeatures == null)
            {
                MatchedFeatures = new MatchedFeature[1];
                MatchedFeatures[0] = feature;

                MatchedAddresses = new RelaxableStreetAddress[1];
                MatchedAddresses[0] = feature.MatchedAddress;

                MatchedFeatureAddresses = new StreetAddress[1];
                MatchedFeatureAddresses[0] = feature.MatchedFeatureAddress;

                //SqlGeography featureGeography = feature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeography;
                //SqlGeography featureGeographyBufferred = featureGeography.STBuffer(10);
                //MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeography = featureGeographyBufferred;

                //KMLDocument kmlDoc = new KMLDocument();
                //string styleName = "style";
                //kmlDoc.AddStyle(styleName, System.Drawing.Color.Blue, 4.0, System.Drawing.Color.Red, true, true);

                //kmlDoc.AddSqlGeography(featureGeography, "feature", styleName, null);
                //kmlDoc.AddSqlGeography(featureGeographyBufferred, "featureBufferred", styleName, null);

                //string kml = kmlDoc.AsString();

                IsExactMatch = feature.IsExactMatch;
                IsRelaxedMatch = feature.IsRelaxedMatch;
                IsSoundexMatch = feature.IsSoundexMatch;
                IsSubstringMatch = feature.IsSubstringMatch;
            }
            else
            {
                MatchedFeature[] temp = new MatchedFeature[MatchedFeatures.Length + 1];
                MatchedFeatures.CopyTo(temp, 0);
                temp[temp.Length - 1] = feature;
                MatchedFeatures = temp;

                RelaxableStreetAddress[] temp2 = new RelaxableStreetAddress[MatchedAddresses.Length + 1];
                MatchedAddresses.CopyTo(temp2, 0);
                temp2[temp2.Length - 1] = feature.MatchedAddress;
                MatchedAddresses = temp2;

                StreetAddress[] temp3 = new StreetAddress[MatchedFeatureAddresses.Length + 1];
                MatchedFeatureAddresses.CopyTo(temp3, 0);
                temp3[temp3.Length - 1] = feature.MatchedFeatureAddress;
                MatchedFeatureAddresses = temp3;

                IsExactMatch = IsExactMatch && feature.IsExactMatch;
                IsRelaxedMatch = IsRelaxedMatch && feature.IsRelaxedMatch;
                IsSoundexMatch = IsSoundexMatch && feature.IsSoundexMatch;
                IsSubstringMatch = IsSubstringMatch && feature.IsSubstringMatch;

            }
        }
    }
}
