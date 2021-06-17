namespace UnSleepingEyeServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Task")]
    public partial class Task
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Task()
        {
            
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        public string Info { get; set; }

        public byte[] Doc { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Act_Date { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date_Start { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date_End { get; set; }

        public bool? Finished { get; set; }
        //[ForeignKey("ID_Stage")]
        public long ID_Stage { get; set; }
        //[ForeignKey("ID_Worker")]
        public long? ID_Worker { get; set; }


        [ForeignKey("ID_Stage")]
        public virtual Stage Stage { get; set; }
        [ForeignKey("ID_Worker")]
        public virtual Worker Worker { get; set; }
    }
}
