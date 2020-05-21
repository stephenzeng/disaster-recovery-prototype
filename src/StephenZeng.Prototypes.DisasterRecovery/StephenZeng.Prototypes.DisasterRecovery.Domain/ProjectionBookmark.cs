using System;
using System.ComponentModel.DataAnnotations;

namespace StephenZeng.Prototypes.DisasterRecovery.Domain
{
    public class ProjectionBookmark
    {
        public int Id { get; set; }

        [MaxLength(36)]
        public string Name { get; set; }

        public long LastEventSequenceId { get; set; }
        public DateTimeOffset LastChangedDate { get; set; }
    }
}