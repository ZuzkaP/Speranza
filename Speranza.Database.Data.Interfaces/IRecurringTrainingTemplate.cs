namespace Speranza.Database.Data
{
    public interface IRecurringTrainingTemplate
    {
        int Capacity { get; set; }
        int Day { get; set; }
        string Description { get; set; }
        int Time { get; set; }
        string Trainer { get; set; }
    }
}