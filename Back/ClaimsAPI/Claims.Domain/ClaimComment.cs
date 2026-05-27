namespace Claims.Domain;

public class ClaimComment
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
