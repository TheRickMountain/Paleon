using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public interface ITradable
    {
        public string GetMarketName();
        public string GetMarketInformation();
        public int GetMarketPrice();
        public MyTexture GetMarketIcon();
    }
}
