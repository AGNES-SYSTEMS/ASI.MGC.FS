using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.MGC.FS.Model.HelperClasses
{
    public class jQueryDataTableParamModel
    {
        public string sEcho { get; set; }
        public string sSearch { get; set; }
        public string sColumns { get; set; }
        public int iDisplayLength { get; set; }
        public int iDisplayStart { get; set; }
        public int iColumns { get; set; }
        public int iSortingCols { get; set; }
    }

}
