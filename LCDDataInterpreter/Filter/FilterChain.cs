using System.Collections.Generic;
using System.Drawing;

namespace LCDDataInterpreter.Filter {
    class FilterChain:IFilter {

        private readonly List<IFilter> filters = new List<IFilter>();

        public FilterChain(params IFilter[] filters) {
            this.filters.AddRange(filters);
        }

        public void Add(IFilter f) {
            filters.Add(f);
        }

        public IList<IFilter> Filters { get { return filters; } }

        public Bitmap Process(Bitmap input, bool returnAsCopy) {
            Bitmap output = returnAsCopy ? input.Clone() as Bitmap : input;

            foreach (var filter in filters) {
                output = filter.Process(output, false);
            }

            return output;
        }
    }
}
