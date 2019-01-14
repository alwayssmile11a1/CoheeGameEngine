using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee.Backend
{
    public interface ICoheeBackend
    {
        string Id { get; }
        string Name { get; }
        int Priority { get; }

        bool CheckAvailable();
        void Init();
        void Shutdown();
    }
}
