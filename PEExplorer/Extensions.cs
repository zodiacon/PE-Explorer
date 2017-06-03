using System;
using System.Collections.Generic;


namespace PEExplorer {
    static class Extensions {
        public static IEnumerable<T> TakeWhileIncluding<T>(this IEnumerable<T> collection, Func<T, bool> predicate) {
            var enumerator = collection.GetEnumerator();
            while(enumerator.MoveNext()) {
                yield return enumerator.Current;
                if(!predicate(enumerator.Current))
                    break;
            }
        }
    }
}
