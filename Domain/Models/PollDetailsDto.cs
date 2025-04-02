using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PollDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Option1Text { get; set; }
        public string Option2Text { get; set; }
        public string Option3Text { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
