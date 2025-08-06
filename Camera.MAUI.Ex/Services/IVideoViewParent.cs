using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.MAUI.Ex.Services
{
    /// <summary>
    /// Ensures that <see cref="Controls.VideoRecordView"/> can 
    /// safely tell it's parent to await the async permissions request it 
    /// makes on first install/run.
    /// </summary>
    public interface IVideoViewParent
    {
        public event Func<Task> NavigatedToAsync;
    }
}
