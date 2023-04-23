using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Exceptions
{
    public class ServiceException: Exception
    {
        string? MsgErrorValidate = null;
        public ServiceException(string msg)
        {
            this.MsgErrorValidate = msg;
        }

        public override string Message
        {
            get { return MsgErrorValidate; }
        }
    }
}
