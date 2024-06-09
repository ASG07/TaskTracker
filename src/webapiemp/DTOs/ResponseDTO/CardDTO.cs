namespace webapiemp.DTOs;

public class CardDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Priority { get; set; }
    public DateTime Date { get; set; }
    public string? State { get; set; }
    public int? AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public int? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public int AuthorAmountWorkingOn { get; set; }
    public int AuthorAmountFinished { get; set; }
    public int AuthorAmountBugsSubmitted { get; set; }
    public int AssigneeAmountWorkingOn { get; set; }
    public int AssigneeAmountFinished { get; set; }
    public int AssigneeAmountBugsSubmitted { get; set; }
}
