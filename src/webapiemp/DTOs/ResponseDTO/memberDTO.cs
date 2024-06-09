namespace webapiemp.DTOs;

public class memberDTO
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string role { get; set; }
    public int AmountWorkingOn { get; set; }
    public int AmountFinished { get; set; }
    public int AmountBugsSubmitted { get; set; }
}