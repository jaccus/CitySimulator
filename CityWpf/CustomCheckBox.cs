using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace City
{
    public class CustomCheckBox : CheckBox
    {
        public bool IsSelected { get; set; }
        public string Label { get; set; }
    }
}
