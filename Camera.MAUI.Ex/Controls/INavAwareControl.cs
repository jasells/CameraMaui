using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.MAUI.Ex.Controls
{
    public interface INavAwareControl
    {
        void OnNavigatedFrom(object sender, EventArgs e);
        void OnNavigatedTo(object sender, EventArgs e);
    }
}
