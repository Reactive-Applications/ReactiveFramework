using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework;
public abstract class NavigableViewModel : ViewModelBase, INavigableViewModel
{
    public virtual void OnNavigatedFrom()
    {

    }

    public virtual void OnNavigatedTo()
    {

    }
}

public abstract class NavigableViewModel<T> : NavigableViewModel, INavigableViewModel<T>
{
    public virtual void OnNavigatedTo(T parameter)
    {

    }
}
