using System.Collections.Generic;
using Unity.Entities;

namespace Components.Singletons.Managed
{
    public class SingletonManagedProvinceNames : IComponentData, INameComponent<string>
    {
        public SingletonManagedProvinceNames()
        {
            Values = new string[1];
            Altered = new Queue<(int, string)>();
        }

        public SingletonManagedProvinceNames(string[] names) : this()
        {
            Values = names;
        }

        public string[] Values { get; set; }
        public Queue<(int, string)> Altered { get; set; }
    }
}