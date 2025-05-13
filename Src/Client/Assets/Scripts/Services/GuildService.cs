using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    class GuildService : Singleton<GuildService>, IDisposable
    {
        public void Init()
        {

        }

        public GuildService()
        {

        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
