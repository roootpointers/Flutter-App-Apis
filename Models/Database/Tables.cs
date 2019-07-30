using System; 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Project.Models.Database
{
    public class Tables { }

    // All the Users Data Table
    public class Peoples
    {
        public int ID { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; } 

        [MaxLength(50)]
        public string Password { get; set; }

        [MaxLength(30)]
        public string Phone { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(10)]
        public string Type { get; set; }
         
        public string Image { get; set; } 
    }

    public class Agendas
    {
        public int ID { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        public DateTime DateTime { get; set; } 

        [MaxLength(200)]
        public string Description { get; set; }
    }

    public class AgendaUsers
    {
        public int ID { get; set; }

        [ForeignKey("Agendas")]
        public int? AgendasID { get; set; }
        public Agendas Agendas { get; set; }

        [ForeignKey("Peoples")]
        public int? User { get; set; }
        public Peoples Peoples { get; set; }

        public bool Status { get; set; }
    }

    public class Notes
    {
        public int ID { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey("Agendas")]
        public int? AgendasID { get; set; }
        public Agendas Agendas { get; set; } 

        [ForeignKey("Peoples")]
        public int? User { get; set; }
        public Peoples Peoples { get; set; }

        [MaxLength(10)]
        public string Status { get; set; }
    }

    public class NotesRequests
    {
        public int ID { get; set; }

        [ForeignKey("Secretary")]
        public int? SecretaryID { get; set; }
        public Peoples Secretary { get; set; }

        [ForeignKey("Notes")]
        public int? NotesID { get; set; }
        public Notes Notes { get; set; }

        [MaxLength(10)]
        public string Status { get; set; }
    } 
}
