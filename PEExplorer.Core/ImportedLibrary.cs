
using System.Collections.Generic;

namespace PEExplorer.Core {
    public class ImportedLibrary {
        public string LibraryName { get; internal set; }

        public ICollection<ImportedSymbol> Symbols { get; } = new List<ImportedSymbol>(16);
    }
}
