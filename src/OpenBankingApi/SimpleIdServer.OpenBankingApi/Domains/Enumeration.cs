using System;

namespace SimpleIdServer.OpenBankingApi.Domains
{
    public class Enumeration : IComparable
    {
        public string Name { get; private set; }
        public int Id { get; private set; }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
