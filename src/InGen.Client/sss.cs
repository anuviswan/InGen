using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace InGen.Client 
{ 
    public partial class AutoNotifyPropertyChangedTest : INotifyPropertyChanged 
    { 
        public event PropertyChangedEventHandler PropertyChanged; 
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "") 
        { 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
        } 
        public string LastName 
        { 
            get => _lastName; 
            set 
            { 
                if (_lastName == value) 
                    return; 
                _lastName = value; NotifyPropertyChanged(); 
            } 
        } 
    } 
}