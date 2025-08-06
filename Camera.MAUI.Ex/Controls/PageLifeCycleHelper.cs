using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.MAUI.Ex.Controls
{
    /// <summary>
    /// Provides helper methods for handling page lifecycle events.
    /// **note: Copied from LibVlcSharp.Maui, but is not working as expected!
    /// </summary>
    internal static class PageLifecycleHelper
    {
        private static readonly WeakEventManager _pageAppearingEventManager = new();
        private static readonly WeakEventManager _pageDisappearingEventManager = new();

        /// <summary>
        /// Occurs when a page is appearing.
        /// </summary>
        public static event EventHandler<PageEventArgs> PageAppearing
        {
            add => _pageAppearingEventManager.AddEventHandler(value);
            remove => _pageAppearingEventManager.RemoveEventHandler(value);
        }

        /// <summary>
        /// Occurs when a page is disappearing.
        /// </summary>
        public static event EventHandler<PageEventArgs> PageDisappearing
        {
            add => _pageDisappearingEventManager.AddEventHandler(value);
            remove => _pageDisappearingEventManager.RemoveEventHandler(value);
        }

        /// <summary>
        /// Registers the page lifecycle events to track page appearing and disappearing.
        /// </summary>
        public static void RegisterPageLifecycleEvents()
        {
            if (Application.Current != null)
            {
                Application.Current.PageAppearing += (s, e) => _pageAppearingEventManager.HandleEvent(s ?? Application.Current, new PageEventArgs(e), nameof(PageAppearing));
                Application.Current.PageDisappearing += (s, e) => _pageDisappearingEventManager.HandleEvent(s ?? Application.Current, new PageEventArgs(e), nameof(PageDisappearing));
            }
        }
    }

    /// <summary>
    /// Represents event data for page-related events. (borrowed from LibVlcSharp.Maui.. need to update to use the 
    /// latest and remove this and our custom VideoView...)
    /// </summary>
    public class PageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the associated <see cref="Page"/> instance.
        /// </summary>
        public Page Page { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageEventArgs"/> class with the specified page.
        /// </summary>
        /// <param name="page">The page associated with the event.</param>
        public PageEventArgs(Page page)
        {
            Page = page;
        }
    }
}
