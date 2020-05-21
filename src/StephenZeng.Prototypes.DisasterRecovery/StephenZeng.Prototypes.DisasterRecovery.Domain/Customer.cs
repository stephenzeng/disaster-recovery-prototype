using System;
using System.ComponentModel.DataAnnotations;

namespace StephenZeng.Prototypes.DisasterRecovery.Domain
{
    public class Customer : EntityBase
    {
        [MaxLength(16)]
        public string FriendlyId { get; set; }

        [MaxLength(256)]
        public string FirstName { get; set; }

        [MaxLength(256)]
        public string Surname { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(32)]
        public string MobilePhone { get; set; }

        [MaxLength(128)]
        public string Email { get; set; }
    }
}