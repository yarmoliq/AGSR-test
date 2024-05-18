using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PatientApi.Models
{
    [Index(nameof(BirthDate))]
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Name Name { get; set; }
        public Gender Gender { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; }
    }
}
