namespace Dav.Ef.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    public class iSwoonDbContext : DbContext
    {
        // Your context has been configured to use a 'iSwoonDbContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Dav.Ef.Database.iSwoonDbContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'iSwoonDbContext' 
        // connection string in the application configuration file.
        public iSwoonDbContext()
            : base("name=iSwoonDbContext")
        {
            
        }
        

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Date> Dates { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<DateEvent> DateEvents { get; set; }
        public virtual DbSet<EventAllowedDateNumber> EventAllowedDateNumbers { get; set; }
    }

    public class Date
    {
        public int Id { get; set; }
        public int DateNumber { get; set; }

        public virtual ICollection<DateEvent> DateEvents { get; set; }
    }

    public class Event
    {
        public int Id { get; set; }
        // this attribute will make it only 100 characters in the databse instead of unlimited (sql data type will be varchar(100) instead of varchar(max))
        [MaxLength(100)]
        public string Description { get; set; }

        public virtual ICollection<EventAllowedDateNumber> AllowedDateNumbers { get; set; }
    }

    public class DateEvent
    {
        [Key]
        [Column(Order = 0)]
        public int DateId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int EventId { get; set; }

        public virtual Date Date { get; set; }
        public virtual Event Event { get; set; }
    }

    public class EventAllowedDateNumber
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int DateNumber { get; set; }

        public virtual Event Event { get; set; }
    }
}