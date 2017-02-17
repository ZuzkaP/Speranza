using Speranza.Models.Interfaces;
using System.Collections.Generic;

namespace Speranza.Models
{
    public enum RecurringTrainingMessages
    {
        NoMessage = 0,
        NoTrainer = 1,
        NoDescription = 2,
        NoCapacity = 3,
        NoModel = 4,
        Success = 5
    }
   public class RecurringModel : IRecurringModel
    {
        public string Trainer { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public IList<bool> IsTrainingInTime { get; set; }
        public RecurringTrainingMessages Message { get; set; }
        public IList<IRecurringTemplateModel> Templates { get; set; }

        public RecurringModel()
        {
            IsTrainingInTime = new List<bool>();
            for (int i = 0; i < 7*13; i++)
            {
                IsTrainingInTime.Add(false);
            }
            Capacity = 10;
            Templates = new List<IRecurringTemplateModel>();
        }

       
    }
}
