using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DbInitializers
{
    public interface IDbInitializer
    {
        void Initialize();
    }
}
