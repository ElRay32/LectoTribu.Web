using LectoTribu.Web.Services;

var builder = WebApplication.CreateBuilder(args);


var baseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7102";

builder.Services.AddRazorPages();


builder.Services.AddHttpClient<IClubsApi, ClubsApi>(c => c.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IUsersApi, UsersApi>(c => c.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IBooksApi, BooksApi>(c => c.BaseAddress = new Uri(baseUrl));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

app.Run();
