using System.Collections.Generic;
using System.Drawing;
using LCDDataInterpreter.Distances;
using LCDDataInterpreter.Filter;

namespace LCDDataInterpreter {
    class ImageMatcher {
        private IList<PatternDefinition> referencePatterns;
        private IDistance distance;
        private IFilter filter;
        
        public ImageMatcher(IList<PatternDefinition> referenceDefinitions, IDistance distance, IFilter filter) {
            this.referencePatterns = referenceDefinitions;
            this.distance = distance;
            this.filter = filter;
        }

        public PatternDefinition Match(Bitmap imgToMatch) {
            PatternDefinition bestMatch = null;
            long dist = long.MaxValue;
            foreach(PatternDefinition d in referencePatterns) {
                distance.calculateDistance(d, filter.Process(imgToMatch, false));
                if(distance.Value < dist) {
                    dist = distance.Value;
                    bestMatch = d;
                }
            }

            return bestMatch;
        }
    }
}
