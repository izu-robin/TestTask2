using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask2.Models
{
    //class DeleteTest
    //{
    //}

    class TestInfo //для списков из названий тестов
    {
        public int id { get; set; }
        public string title { get; set; }

        public TestInfo(string title)
        {
            this.title = title;
        }

        public TestInfo() { }

    }
}
