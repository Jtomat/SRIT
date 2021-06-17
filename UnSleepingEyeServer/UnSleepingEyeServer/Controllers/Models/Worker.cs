namespace UnSleepingEyeServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Worker")]
    public partial class Worker
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Worker()
        {
            Task = new HashSet<Task>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        public Guid ID_user { get; set; }

        public int? ID_Role { get; set; }
        [ForeignKey("ID_user")]
        public virtual AppUser User { get; set; }

        public virtual ICollection<Task> Task { get; set; }
        [ForeignKey("ID_Role")]
        public virtual WorkerRole WorkerRole { get; set; }
    }
}
