using ServerWinForm.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWinForm.Data
{
    public class Proposal
    {
        public Guid Id { get; private set; }
        public DateTime DataTime { get; private set; }
        public List<Product> Products { get; private set; }
        public ProposalStatus Status { get; set; }

        public Proposal(List<Product> products)
        {
            Id = Guid.NewGuid();
            DataTime = DateTime.Now;
            Products = products;
            Status = ProposalStatus.UNPROCESSED;
        }

    }
}
