using ServerWinForm.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServerWinForm.Data
{
    public class Proposal
    {
        public string Id { get; private set; }
        public DateTime DateTime { get; private set; }
        public List<Product> Products { get; private set; }
        public ProposalStatus Status { get; set; }

        public Proposal(string proposalId, List<Product> products)
        {
            Id = proposalId;
            DateTime = DateTime.Now;
            Products = products;
            Status = ProposalStatus.UNPROCESSED;
        }

        [JsonConstructor]
        public Proposal(string Id, DateTime DateTime, List<Product> Products, ProposalStatus Status)
        {
            this.Id = Id;
            this.DateTime = DateTime;
            this.Products = Products;
            this.Status = Status;
        }

    }
}
