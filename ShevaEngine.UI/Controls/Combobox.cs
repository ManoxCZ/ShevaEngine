using System;
using System.Collections;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Combobox.
    /// </summary>
    public class Combobox : Control
    {
        public BehaviorSubject<IList> ItemsSource { get; private set; }
        public BehaviorSubject<object> SelectedItem { get; private set; }        


        /// <summary>
        /// Constructor.
        /// </summary>
        public Combobox()
        {
            ItemsSource = CreateProperty<IList>(nameof(ItemsSource), null);
            SelectedItem = CreateProperty<object>(nameof(SelectedItem), null);

            Disposables.Add(ItemsSource
                .CombineLatest(SelectedItem, (itemsSource, selectedItem) => (itemsSource, selectedItem))
                .Subscribe(item =>
                {
                    if (item.itemsSource == null && item.selectedItem is Enum enumValue)
                        ItemsSource.OnNext(Enum.GetValues(enumValue.GetType()));                    
                }));
        }
    }
}
