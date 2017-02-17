using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models.Interfaces
{
     public interface IRecurringTemplateModel
    {
        int Capacity { get; set; }
        int Day { get; set; }
        string Description { get; set; }
        int Time { get; set; }
        string Trainer { get; set; }
    }
}
