namespace UnSleepingEyeServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OperationReport")]
    public partial class OperationReport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OperationReport()
        {
            ReportData = new HashSet<ReportData>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        public long ID_Stage { get; set; }

        public DateTime? Date { get; set; }

        [Required]
        [StringLength(128)]
        public string Type { get; set; }

        public string Info { get; set; }

        public byte[] Doc { get; set; }
        [ForeignKey("ID_Stage")]
        public virtual Stage Stage { get; set; }

        public virtual ICollection<ReportData> ReportData { get; set; }
    }
}
