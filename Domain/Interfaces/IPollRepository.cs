using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPollRepository
    {
        void CreatePoll(Poll poll);
        IQueryable<object> GetPolls(int? id = null);
        void Vote(int pollId, int chosenOption);
    }
}