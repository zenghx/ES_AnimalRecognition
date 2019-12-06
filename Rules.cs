using System;
using System.Collections.Generic;

namespace ESAnimalRecognition
{
    class Rules
    {
        public List<String> allFeatures { get; set; }
        public List<String> finalConclusion { get; set; }
        public HashSet<Rule> rules { get; set; }
    }

    class Rule
    {
        public HashSet<int> features { get; set; }
        public int conclusion { get; set; }
        public bool isFinal { get; set; }
        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var target= obj as Rule;
            if (features.Count != target.features.Count)
                return false;
            foreach (var feature in target.features)
            {
                if(!features.Contains(feature))
                    return false;
            }
            if (conclusion != target.conclusion)
                return false;
            if (isFinal != target.isFinal)
                return false;
            return true;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            int sum = 0;
            foreach (var item in features)
            {
                sum += item;
            }
            return conclusion+sum;
        }
    }
}
