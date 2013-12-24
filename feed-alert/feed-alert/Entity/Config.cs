using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feed_alert.Entity
{
    using System.Runtime.Serialization;

    [DataContractAttribute]
    class Config
    {
        [DataMember]
        public int UpdateInterval { get; set; }

    }
}
