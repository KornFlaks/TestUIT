using System.Collections.Generic;

namespace Components.Singletons.Managed
{
    public interface INameComponent<TListed>
    {
        public TListed[] Values { get; set; }
        public Queue<(int, TListed)> Altered { get; set; }
    }
}