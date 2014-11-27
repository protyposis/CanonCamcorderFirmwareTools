using System.Drawing;

namespace LCDDataInterpreter.Distances {
    interface IDistance {
        long Value { get; }
        void calculateDistance(PatternDefinition d, Bitmap b);
    }
}
