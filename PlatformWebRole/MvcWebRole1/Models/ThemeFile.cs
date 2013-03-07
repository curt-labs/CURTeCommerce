using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommercePlatform.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;

namespace EcommercePlatform {
    partial class ThemeFile {

        public string Render() {
            return this.ThemeFileType.structure.Replace("[path]",this.filePath);
        }

    }
}