using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Common.Provider
{
    public interface IProvider
    {
        ProviderManager Parent { get; set; }
    }
}
