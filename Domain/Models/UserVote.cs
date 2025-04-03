using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class UserVote
    {
        [ForeignKey("CustomUser")]
        public string UserId { get; set; }

        [ForeignKey("Poll")]
        public int PollId { get; set; }

        public virtual CustomUser User { get; set; } 
        public virtual Poll Poll { get; set; } 
    }
}