using System;
using System.Threading.Tasks;
using TinyCqrs;

namespace TinyCQRS.Autofac.Tests.Support
{
    public class Query1 : IQuery<Query1Args, string>
    {
        public Task<string> Perform(Query1Args input)
        {
            throw new NotImplementedException();
        }
    }
}