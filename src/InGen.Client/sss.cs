
//    using System;
//    using System.ComponentModel;
//    using System.Runtime.CompilerServices;
//namespace Client
//{
//    public partial class AutoNotifyGeneratorTests : INotifyPropertyChanged
//    {

//        public event PropertyChangedEventHandler PropertyChanged;

//        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//        public String FirstName
//        {
//            get => _firstName;
//            set
//            {
//                if (_firstName == value) return;
//                _firstName = value;
//                NotifyPropertyChanged();
//            }
//        }
//        public String LastName
//        {
//            get => _lastName;
//            set
//            {
//                if (_lastName == value) return;
//                _lastName = value;
//                NotifyPropertyChanged();
//            }
//        }
//    }
//}
