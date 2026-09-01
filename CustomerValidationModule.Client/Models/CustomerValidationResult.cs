namespace CustomerValidationModule.Client.Models;

public class CustomerValidationResult
{
    public string NationalCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public bool IsValidIdentity { get; set; }
    public int RiskScore { get; set; } 
    public string RiskLevel { get; set; } = string.Empty; 
    public string CreditStatus { get; set; } = string.Empty;
    public List<string> Alerts { get; set; } = new();
    public DateTime InquiryTime { get; set; } = DateTime.Now;
}
