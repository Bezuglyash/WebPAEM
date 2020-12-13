using System.Collections.Generic;

namespace NetMarket.Models
{
    /// <summary>
    /// Модель данных "Права доступа"
    /// </summary>
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<People> People { get; set; }

        public Role()
        {
            People = new List<People>();
        }
    }
}