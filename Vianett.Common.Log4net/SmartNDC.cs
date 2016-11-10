using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vianett.Common.Log4net
{
    public class SmartNDC : IDisposable
    {
        IDisposable ndc;

        public SmartNDC(string message)
        {
            if (message != null)
            {
                ndc = log4net.NDC.Push(message);
            }
            else
            {
                ndc = null;
            }
        }

        public void Dispose()
        {
            if (ndc != null)
            {
                ndc.Dispose();
                ndc = null;
            }
        }
    }
}
