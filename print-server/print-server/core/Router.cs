using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    class Router
    {
        private String[] LocalPath;
        public Router(String path)
        {
            this.LocalPath = path.Split(new char[] { '/' });
        }

        public String getController()
        {
            if(LocalPath.Length >= 2) {
                return LocalPath[1];
            } else {
                return "Home";
            }
        }

        public String getAction()
        {
            if (LocalPath.Length >= 3)
            {
                return LocalPath[2];
            }
            else
            {
                return "Index";
            }
        }
    }
}
