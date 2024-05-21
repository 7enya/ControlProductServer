using ServerWinForm.Enums;
using System.Text.Json.Serialization;

namespace ServerWinForm.Data
{
    public class Proposal
    {
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonIgnore]
        public DateTime DateTime { get; private set; }

        [JsonPropertyName("products")]
        public List<Product> Products { get; private set; }

        [JsonIgnore]
        public ProposalStatus Status { get; set; }

        [JsonConstructor]
        public Proposal(string id, List<Product> products)
        {
            Id = id;
            DateTime = DateTime.Now;
            Products = products;
            Status = ProposalStatus.UNPROCESSED;
        }
    }
}