using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework;
public interface INavigableViewModel : IViewModel
{
    void OnNavigatedTo();
    void OnNavigatedFrom();
}

public interface INavigableViewModel<T> : INavigableViewModel
{
    void OnNavigatedTo(T parameter);
}
