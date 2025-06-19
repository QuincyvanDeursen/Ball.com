using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PaymentService.Domain
{
    public class Payment
    {
        public Guid Id { get; init; }
        public decimal TotalPrice { get; set; }
        public PaymentStatus Status { get; set; }
        public CustomerSnapshot Customer { get; set; }
    }

    //Deze customer snapshot is een owned entity, wat betekent dat het geen aparte tabel heeft in de database, maar wordt opgeslagen in de Payment tabel zelf.
    // Dit zorgt ervoor dat payments altijd de informatie bevat over de klant op het moment van betaling.
    // Wijzigingen aan klantgegevens worden niet doorgevoerd in de Payment tabel, omdat dit de integriteit van de data zou kunnen aantasten.

    [Owned]
    public class CustomerSnapshot
    {
        public Guid Id { get; init; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        public string Email { get; set; }

    }
}
