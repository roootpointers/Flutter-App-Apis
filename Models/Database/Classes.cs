using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models.Database
{
    public class Classes
    {
    }
    // All the Users Data Table
    public class PeoplesClass
    {
        public int ID { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(10)]
        public string Type { get; set; }

        [MaxLength(500)]
        public string Image { get; set; }
    }

    public class AgendasClass
    {
        public int ID { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(20)]
        public string DateTime { get; set; } 

        [MaxLength(200)]
        public string Description { get; set; }

        public int Users { get; set; }

        [MaxLength(30)]
        public string Color { get; set; }

        [MaxLength(10)]
        public string Status { get; set; }
    }

    //Agenda for Member
    public class AgendasClassForMember
    {
        public int ID { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(20)]
        public string DateTime { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public int Users { get; set; } 

        [MaxLength(10)]
        public string AgendaStatus { get; set; }

        [MaxLength(30)]
        public string AgendaStatusColor { get; set; }

        [MaxLength(10)]
        public string MemberStatus { get; set; }

        [MaxLength(30)]
        public string MemberStatusColor { get; set; }
    }

    public class AgendaUsersClass
    {
        public int ID { get; set; } 

        public int? AgendasID { get; set; } 
         
        public int? UserID { get; set; } 

        public bool Status;
    }

    public class NotesClass
    {
        public int ID { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; } 

        public int? AgendasID { get; set; } 
         
        public int? UserID { get; set; } 

        //1 for public 0 for private
        public int Status { get; set; }
    }

    public class NotesRequestsClass
    {
        public int ID { get; set; }
         
        public int? SecretaryID { get; set; } 
         
        public int? NotesID { get; set; } 

        //0 for Approve 1 for disaprove
        public int Status { get; set; }
    }

    public class ResponseObject
    {
        public bool Status { get; set; }

        public string Message { get; set; }

        public Object Data { get; set; }
    }

    public class Response
    {
        public bool Status { get; set; }

        public string Message { get; set; } 
    }

    //Class for Agenda For Admin
    public class ResponseList
    {
        public bool Status { get; set; }

        public string Message { get; set; }

        public IEnumerable<Object> Data { get; set; }
    }

    //Class for Agenda For Admin 
    public class SingleAgenda
    {
        public bool Status { get; set; }

        public string Message { get; set; }

        public Object Agenda { get; set; }

        public IEnumerable<Object> Secretary { get; set; }

        public IEnumerable<Object> Members { get; set; }

        public Object Note { get; set; }
    } 

    public class ChangePassword
    {
        public int ID { get; set; }

        [MaxLength(50)]
        public string OldPassword { get; set; }

        [MaxLength(50)]
        public string NewPassword { get; set; }
    }

    public class Login
    { 
        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }

        [MaxLength(10)]
        public string Type { get; set; }
    }
}