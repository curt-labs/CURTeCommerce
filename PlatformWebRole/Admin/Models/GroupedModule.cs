using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class GroupedModule : Module {

        public List<Module> subs { get; set; }
        public int hasAccess { get; set; }
    }
}