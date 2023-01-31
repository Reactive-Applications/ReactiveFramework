using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// In Projekten im SDK-Stil wie dem vorliegenden, bei dem verschiedene Assemblyattribute
// üblicherweise in dieser Datei definiert wurden, werden diese Attribute jetzt während
// der Builderstellung automatisch hinzugefügt und mit Werten aufgefüllt, die in den
// Projekteigenschaften definiert sind. Informationen dazu, welche Attribute einbezogen
// werden und wie dieser Prozess angepasst werden kann, finden Sie unter https://aka.ms/assembly-info-properties.


// Wenn "ComVisible" auf FALSE festgelegt wird, sind die Typen in dieser Assembly
// für COM-Komponenten nicht sichtbar. Wenn Sie von COM aus auf einen Typ in dieser
// Assembly zugreifen müssen, legen Sie das ComVisible-Attribut für den betreffenden
// Typ auf TRUE fest.

[assembly: ThemeInfo(
    ResourceDictionaryLocation.ExternalAssembly, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]


[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM
// bereitgestellt wird.

[assembly: Guid("30547ad7-cdc7-4780-bb03-2503b9dba813")]

[assembly: XmlnsDefinition("https://reactivesolution.de/RxFramework/WPF", "RxApplication.WPF.FluentControls.Windowing")]
[assembly: XmlnsDefinition("https://reactivesolution.de/RxFramework/WPF", "RxApplication.WPF.FluentControls.Text")]
[assembly: XmlnsDefinition("https://reactivesolution.de/RxFramework/WPF.Behaviors", "RxApplication.WPF.FluentControls.Behaviors")]