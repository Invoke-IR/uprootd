using System;
using System.ServiceProcess;

namespace Uprootd
{
    internal static class Program
    {
        private static void Main()
        {
            ServiceBase[] services = new ServiceBase[]
			{
				new Uprootd()
			};
            ServiceBase.Run(services);
        }
    }
}
