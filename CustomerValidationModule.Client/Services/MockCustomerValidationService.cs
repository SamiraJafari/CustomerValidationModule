using CustomerValidationModule.Client.Models;

namespace CustomerValidationModule.Client.Services;

public class MockCustomerValidationService : ICustomerValidationService
{
    private readonly Random _random = new();

    public async Task<CustomerValidationResult> GetCustomerValidationAsync(string nationalCode, CancellationToken cancellationToken = default)
    {
        var delay = _random.Next(400, 1200);
        await Task.Delay(delay, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(nationalCode) || nationalCode.Length != 10 || !nationalCode.All(char.IsDigit))
        {
            throw new ArgumentException("کد ملی باید دقیقاً ۱۰ رقم باشد.", nameof(nationalCode));
        }

        if (nationalCode.StartsWith("000"))
        {
            throw new HttpRequestException("مشتری با این کد ملی یافت نشد.", null, System.Net.HttpStatusCode.NotFound);
        }

        if (nationalCode.StartsWith("999"))
        {
            throw new HttpRequestException("خطای داخلی سرور در سرویس اعتبارسنجی.", null, System.Net.HttpStatusCode.InternalServerError);
        }

        if (nationalCode.StartsWith("888"))
        {
            await Task.Delay(5000, cancellationToken); 
            throw new TimeoutException("زمان پاسخ‌گویی سرویس اعتبارسنجی به پایان رسید.");
        }

        var riskScore = _random.Next(5, 95);
        var riskLevel = riskScore switch
        {
            < 30 => "Low",
            < 60 => "Medium",
            _ => "High"
        };

        return new CustomerValidationResult
        {
            NationalCode = nationalCode,
            FullName = GetRandomPersianName(),
            FatherName = GetRandomPersianFatherName(),
            BirthDate = DateTime.Today.AddYears(-_random.Next(25, 65)).AddDays(-_random.Next(0, 365)),
            Gender = _random.Next(2) == 0 ? "مرد" : "زن",
            IsValidIdentity = true,
            RiskScore = riskScore,
            RiskLevel = riskLevel,
            CreditStatus = riskScore < 40 ? "مطلوب" : riskScore < 70 ? "متوسط" : "نیاز به بررسی",
            Alerts = GenerateAlerts(riskScore),
            InquiryTime = DateTime.Now
        };
    }

    private static string GetRandomPersianName()
    {
        var names = new[]
        {
            "علی محمدی", "سارا احمدی", "رضا کریمی", "مریم حسینی",
            "حسین رضایی", "فاطمه نوری", "امیر عباسی", "زهرا موسوی",
            "محمد جعفری", "نرگس صادقی"
        };
        return names[Random.Shared.Next(names.Length)];
    }

    private static string GetRandomPersianFatherName()
    {
        var names = new[] { "احمد", "محمود", "حسین", "علی", "رضا", "محمد", "حسن", "جواد" };
        return names[Random.Shared.Next(names.Length)];
    }

    private static List<string> GenerateAlerts(int riskScore)
    {
        var alerts = new List<string>();
        if (riskScore > 70)
            alerts.Add("نمره ریسک بالا – نیاز به تأیید مدیر شعبه");
        if (riskScore > 50)
            alerts.Add("سابقه چک برگشتی در سیستم‌های استعلامی");
        if (riskScore < 20)
            alerts.Add("مشتری با اعتبار عالی");
        return alerts;
    }
}
