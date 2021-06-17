namespace UnSleepingEyeServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Stage")]
    public partial class Stage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Stage()
        {
            Task = new HashSet<Task>();
            OperationReport = new HashSet<OperationReport>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }
        
        public long ID_Project { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date_Start { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date_End { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Actual_Date_End { get; set; }

        public bool? Finished { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OperationReport> OperationReport { get; set; }
        [ForeignKey("ID_Project")]
        public virtual Project Project { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Task> Task { get; set; }
    }
}
