using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectLibrary
{
    public static class Template_Feature
    {
        static List<Feature> _list;

        static Template_Feature()
        {
            _list = new List<Feature>();
            _list.Add(new Feature() { featureDiscipline = Discipline.Electrical, featureCategory = "General", featureTypeID = FeatureTypeID.SelectTestEq, featureName = "Select Test Equipment" });
        }

        public static List<Feature> GetList()
        {
            return _list;
        }
    }

    public class Feature
    {
        public Feature()
        {
            featureExists = false;
        }

        public Discipline featureDiscipline { get; set; }
        public string featureCategory { get; set; }
        public FeatureTypeID featureTypeID { get; set; } //use typeID because display name might need to be change in the future
        public string featureName { get; set; }
        public bool featureExists { get; set; }
        public override string ToString()
        {
            return featureName;
        }
    }
}
