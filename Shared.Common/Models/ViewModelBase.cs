using System.ComponentModel;
using System.Runtime.CompilerServices;
using Shared.Common.Properties;

namespace Shared.Common.Models
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual string this[string columnName] { get { return null; } }

        public string Error { get; }
    }
}