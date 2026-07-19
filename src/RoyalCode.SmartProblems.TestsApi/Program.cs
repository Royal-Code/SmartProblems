using RoyalCode.SmartProblems.TestsApi.Apis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddProblemDetailsDescriptions(options =>
{
    options.Descriptor.AddFromJsonFile("problem-details.json");
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddContactsSample();

var app = builder.Build();
app.EnsureContactsDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapProblems();
app.MapProblemDetailsDescriptionPage();
app.MapMatchApi();
app.MapAcceptedApi();
app.MapContactsApi();

app.MapControllers();

app.Run();
