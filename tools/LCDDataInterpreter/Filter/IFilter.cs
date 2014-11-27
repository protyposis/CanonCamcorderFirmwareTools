using System.Drawing;

namespace LCDDataInterpreter.Filter {
    interface IFilter {
        Bitmap Process(Bitmap input, bool returnAsCopy);
    }
}
