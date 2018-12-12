using System.ComponentModel;

namespace DotNet.Core.Demo.Utilities.ComponentModel
{
    public class CustomerDescriptionAttribute : DescriptionAttribute
    {
        public CustomerDescriptionAttribute(string description, params string[] mutexIds)
            : this(description, true, 0, mutexIds)
        {
        }

        public CustomerDescriptionAttribute(string description, int type, params string[] mutexIds)
            : this(description, true, type, 0, mutexIds)
        {
        }

        public CustomerDescriptionAttribute(string description, bool display, params string[] mutexIds)
            : this(description, display, 0, mutexIds)
        {
        }

        public CustomerDescriptionAttribute(string description, bool display, int type, params string[] mutexIds)
            : this(description, display, type, 0, mutexIds)
        {
        }

        /*public CustomerDescriptionAttribute(string description, int sort, params string[] mutexIds)
            : this(description, true, sort, mutexIds)
        {
        }*/

        public CustomerDescriptionAttribute(string description, int type, int sort, params string[] mutexIds)
            : this(description, true, type, sort, mutexIds)
        {
        }

        /*public CustomerDescriptionAttribute(string description, bool display, int sort, params string[] mutexIds)
            : this(description, display, default(Enum) , sort, mutexIds)
        {
        }*/

        public CustomerDescriptionAttribute(string description, bool display, int type, int sort, params string[] mutexIds)
            : base(description)
        {
            Display = display;
            Type = type;
            Sort = sort;
            MutexIds = mutexIds ?? new[] {"default"};
        }

        public bool Display { get; }

        public int Sort { get; }

        public int Type { get; }

        public string[] MutexIds { get; }
    }
}
