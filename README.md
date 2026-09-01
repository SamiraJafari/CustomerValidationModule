## خلاصه پروژه
این پروژه یک کامپوننت Blazor WebAssembly است که به کارشناس شعبه اجازه می‌دهد با وارد کردن **کد ملی** مشتری، وضعیت اعتبارسنجی، نمره ریسک و اطلاعات هویتی را به صورت زنده مشاهده کند.


## ساختار پروژه

```
CustomerValidationModule/
├── CustomerValidationModule.sln
├── CustomerValidationModule/                 # Host (Server)
│   └── Components/Pages/Home.razor           # صفحه اصلی
└── CustomerValidationModule.Client/          # WebAssembly Client
    ├── Components/
    │   ├── CustomerLookup.razor              # کامپوننت اصلی
    │   └── CustomerLookup.razor.css          # CSS Isolation
    ├── Models/
    │   └── CustomerValidationResult.cs
    ├── Services/
    │   ├── ICustomerValidationService.cs
    │   └── MockCustomerValidationService.cs  # سرویس شبیه‌ساز
    └── Program.cs
```

---

## ویژگی‌های پیاده‌سازی‌شده

### ۱. فیلد ورودی کد ملی
- محدودیت ۱۰ رقم
- فقط اعداد (`inputmode="numeric"`)
- اعتبارسنجی سمت کلاینت
- غیرفعال شدن در حالت Loading

### ۲. سرویس شبیه‌ساز (Mock Service)

| `000xxxxxxx`  ->  مشتری یافت نشد - 404    
| `999xxxxxxx`  ->  خطای سرور - 500     
| `888xxxxxxx` ->   Timeout   
| Other 10 digits ->  موفقیت با داده تصادفی - 200 

تأخیر تصادفی ۴۰۰–۱۲۰۰ میلی‌ثانیه برای شبیه‌سازی شبکه واقعی.

### ۳. وضعیت‌های رابط کاربری (UI States)
- **Idle**: حالت اولیه – پیام راهنما
- **Loading**: اسپینر + انیمیشن نقطه + غیرفعال کردن ورودی
- **Success**: نمایش کامل اطلاعات + نمره ریسک + هشدارها
- **Error**: پیام مناسب برای هر نوع خطا + دکمه «تلاش مجدد»

### ۴. مدیریت ترافیک و Debounce
- Debounce با تأخیر **۴۵۰ میلی‌ثانیه**
- هر کاراکتر تایپ‌شده درخواست جدید نمی‌فرستد
- با فشردن Enter جستجو فوری انجام می‌شود

**Why Debounce؟**  
جلوگیری از سیل درخواست به سرویس‌های استعلامی گران‌قیمت و کاهش فشار روی شبکه و سرور.

### ۵. لغو درخواست‌های قدیمی (Race Condition & Cancellation)
- استفاده از `CancellationTokenSource` جداگانه برای Debounce و برای Request
- با هر تغییر جدید در ورودی، درخواست قبلی لغو می‌شود
- نتیجه درخواست لغوشده نادیده گرفته می‌شود

**Why Cancellation؟**  
اگر کاربر سریع کد را عوض کند، ممکن است پاسخ درخواست قدیمی‌تر بعد از جدیدتر برسد و UI را خراب کند (Race Condition).

### ۶. مدیریت حافظه و طول‌عمر کامپوننت
- کامپوننت `IDisposable` را پیاده‌سازی کرده
- در `Dispose()` هر دو `CancellationTokenSource` را Cancel و Dispose می‌کند
- جلوگیری از Memory Leak هنگام Unmount شدن کامپوننت

### ۷. طراحی واکنش‌گرا و CSS Isolation
- فایل `CustomerLookup.razor.css` (Blazor CSS Isolation)
- Grid دو ستونه در دسکتاپ، یک ستونه در موبایل
- رنگ‌بندی معنایی (سبز = موفقیت، قرمز = خطا، نارنجی = هشدار)

### ۸. تفکیک وظایف (Separation of Concerns)
- **Model**: فقط داده
- **Service Interface + Mock**: منطق استعلام
- **Component**: فقط UI و مدیریت state
- ثبت سرویس در DI Container (`Program.cs`)

---

## نکات طراحی (Trade-offs)

| تصمیم | دلیل | جایگزین ممکن |
|-------|------|--------------|
| Debounce سمت کلاینت | کاهش ترافیک و تجربه کاربری روان | Debounce سمت سرور (پیچیده‌تر) |
| CancellationToken | جلوگیری از Race Condition | صف درخواست‌ها (پیچیده‌تر و کندتر) |
| Mock Service | توسعه مستقل از سرویس واقعی | اتصال مستقیم به API واقعی |
| CSS Isolation | جلوگیری از تداخل استایل‌ها | فایل CSS سراسری |
| WebAssembly | تعامل‌پذیری بالا و آفلاین نسبی | Blazor Server (نیاز به SignalR مداوم) |

