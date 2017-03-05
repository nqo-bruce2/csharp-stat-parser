using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Models
{
    public class Player
    {       
        public int Id { get; set; }
        [Index(IsUnique = true)]
        [StringLength(20)]
        public string IpAddress { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
