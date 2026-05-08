using Spyro_Editor.Data.Level;
using System.Collections.ObjectModel;

namespace Spyro_Editor.Models
{
    public class MobyInstancesModel
    {
        public ObservableCollection<MobyInstanceItem> Items = new();

        public void Load(MobyInstance[] instances)
        {
            Items.Clear();
            foreach(MobyInstance instance in instances)
            {
                Items.Add(new MobyInstanceItem(instance));
            }
        }
    }

    public class MobyInstanceItem
    {
        public string Title;
        public string Subtitle;
        public string Subtitle2;

        public MobyInstanceItem(MobyInstance instance)
        {
            Title = "Class ID: " + instance.ClassID.ToString();
            Subtitle = $"Position: {instance.X}, {instance.Y}, {instance.Z}";
            Subtitle2 = $"Rotation: {instance.Yaw}";
        }
    }
}
