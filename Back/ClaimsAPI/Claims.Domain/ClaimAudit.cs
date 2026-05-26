namespace Claims.Domain;

public class ClaimAudit
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public string FieldChanged { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; }
}
