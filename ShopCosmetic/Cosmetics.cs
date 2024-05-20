using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopCosmetic
{
    public class Cosmetics : CosmeticsEntities
    {
        public static CosmeticsEntities context;
        public static CosmeticsEntities GetContext()
        {
            if (context == null)
                context = new CosmeticsEntities();
            return context;
        }

    }
}
