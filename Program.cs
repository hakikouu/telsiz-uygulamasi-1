using TelsizSohbetUygulamasi.Hubs; // ChatHub'ı bulabilmesi için bu satırı ekledik

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR(); // 1. YENİ SATIR: SignalR servisini projeye ekliyoruz.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub"); // 2. YENİ SATIR: ChatHub'ımızı "/chatHub" adresine bağlıyoruz.

app.Run(Environment.GetEnvironmentVariable("PORT") ?? "8080");