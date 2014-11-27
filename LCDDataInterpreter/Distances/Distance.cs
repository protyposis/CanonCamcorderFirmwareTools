using System.Drawing;

namespace LCDDataInterpreter.Distances {
    abstract class Distance: IDistance {
        public long Value { get; protected set; }

        #region IDistance Member

        public abstract void calculateDistance(PatternDefinition d, Bitmap b);

        #endregion
    }
}
