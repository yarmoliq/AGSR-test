using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientApi.Models
{
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Name Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; }
    }
}
