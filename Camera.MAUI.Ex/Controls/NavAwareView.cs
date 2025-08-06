using System.Diagnostics;

namespace Camera.MAUI.Ex.Controls;

public abstract class NavAwareView : ContentView
{
    /// <summary>
    /// Located when the parent is set, <c>null</c> until then.
    /// **todo/improvement: make this property raise PropertyChanging and PropertyChanged events
    /// </summary>
    public Page ParentPage { get; protected set; } = null;

    public NavAwareView() {}

    #region locate the root page

    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (Parent is Page p)
        {
            SetupParentPage(p);
        }
        else
        {
            Debug.WriteLine($"======= in {nameof(NavAwareView.OnParentSet)}: {nameof(NavAwareView)} ParentPage should be set before using VideoRecordView.");

            // have to find the root parent page, but the view tree is built leaf-up, not root down...
            if (Parent.Parent == null)
                Parent.ParentChanged += ParentPageHandler;
            else
                ParentPageHandler(Parent.Parent, EventArgs.Empty);
        }
    }

    protected virtual void SetupParentPage(Page parent)
    {
        if (ParentPage == null)
        {
            ParentPage = parent;
        }
    }

    protected virtual void ParentPageHandler(object sender, EventArgs e)
    {
        var newParent = sender as Element;

        Debug.WriteLine($"======= {nameof(NavAwareView)}.Parent parent changed: {sender?.GetType().Name}");

        // remove this handler
        if (newParent != null)
        {
            newParent.ParentChanged -= ParentPageHandler;
        }

        if (newParent is Page p)
        {
            Debug.WriteLine($"======= {nameof(NavAwareView)}.ParentPage found: {p.GetType()}.");
            SetupParentPage(p);
        }
        else if (newParent?.Parent is Page p2)
        {
            Debug.WriteLine($"======= {nameof(NavAwareView)}.ParentPage found via .Parent: {p2.GetType()}.");
            SetupParentPage(p2);
        }
        else if (newParent.Parent == null)
        {
            Debug.WriteLine($"======= {nameof(NavAwareView)} ParentPage should be set before using, looking farther up UI tree.");
            newParent.ParentChanged += ParentPageHandler;
        }
        else
        {
            ParentPageHandler(newParent.Parent, EventArgs.Empty);
        }
    }
    #endregion

    #region handle OnNavigated events
    protected abstract Task OnNavigatedTo(Page page, EventArgs e);
    //protected abstract Task OnNavigatedFrom(Page page, EventArgs e);
    protected abstract Task OnNavigatingFrom(Page page, EventArgs e);

    protected abstract void OnDisappearing(Page page, EventArgs e);

    protected abstract void OnAppearing(Page page, EventArgs e);
    #endregion
}