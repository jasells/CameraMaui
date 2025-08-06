using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Video_Demo.ViewModel;

public class BaseViewModel : Camera.MAUI.Ex.NotifyPropertyChangedBase, IQueryAttributable, INotifyPropertyChanged
{

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        PopulateQueryAttributes(query);
    }
    
    protected virtual void PopulateQueryAttributes(IDictionary<string, object> query) { }
    
    /// <summary>
    /// This is where we handle the hardware back button press event on android
    /// <para>true - Prevent navigation</para>
    /// <para>false - Allow navigation</para>
    /// </summary>
    public virtual bool OnBackButtonPressed()
    {
        return false;
    }
}