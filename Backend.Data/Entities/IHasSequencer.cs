namespace Backend.Data.Entities
{
    public interface IHasSequencer
    {
        int id { get; set; }
        string getSequenser();
    }
}