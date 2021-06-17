namespace UnSleepingEyeServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    //[Table("Project")]
    public partial class Project
    {
        public Project()
        {
            Stage = new HashSet<Stage>();
        }

        public long ID { get; set; }

        public string Name { get; set; }

        public string Info { get; set; }

        public byte[] Act_end { get; set; }

        public byte[] Act_confirmed { get; set; }

        public virtual ICollection<Stage> Stage { get; set; }
    }
}
