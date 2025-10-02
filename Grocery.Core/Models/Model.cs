using CommunityToolkit.Mvvm.ComponentModel;

namespace Grocery.Core.Models
{
    public abstract partial class Model : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty] private string _name; // ✅ backing field, genereert publieke property 'Name'

        public Model(int id, string name)
        {
            Id = id;
            _name = name;
        }
    }
}
