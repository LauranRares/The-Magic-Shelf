using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Models
{
    public class UserHistory
    {
        public int Id { get; set; }

        public string TheUserId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public DateTime OrderTime { get; set; }

        public string Name { get; set; }
    }
}
