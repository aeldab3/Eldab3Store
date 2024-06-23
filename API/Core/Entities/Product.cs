using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }
    }
}
