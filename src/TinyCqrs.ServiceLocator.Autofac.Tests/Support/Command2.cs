using System;
using System.Threading.Tasks;
using TinyCqrs;

namespace TinyCQRS.Autofac.Tests.Support
{
    public class Command2 : ICommand<Command1Args>
    {
        public Task Invoke(Command1Args command)
        {
            throw new NotImplementedException();
        }
    }
}