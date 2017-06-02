

namespace PEExplorer.Core {
    public class ExportedSymbol {
        public string Name { get; set; }
        public int Ordinal { get; set; }
        public uint Address { get; set; }
        public string ForwardName { get; set; }
        public string UndecoratedName { get; set; }
    }
}
