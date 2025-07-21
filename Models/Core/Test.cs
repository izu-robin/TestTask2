using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TestTask2.Models.Core
{
    class Test
    {
        public int id { get; set; }
        public string title { get; set; }

        public Test(string title)
        {
            this.title = title;
        }

        public Test() {}

    }
}
