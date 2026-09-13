using DocuFlow.Api.Configuration;
using DocuFlow.Api.Features.Documents;
using DocuFlow.Api.Features.Uploads;
using DocuFlow.Api.Services;
using DocuFlow.Aws.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AwsOptions>(builder.Configuration.GetSection("Aws"));
builder.Services.AddDocuFlowAws(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CreateUploadValidator>();
builder.Services.AddScoped<PresignedUrlService>();
builder.Services.AddScoped<CreateUploadHandler>();
builder.Services.AddScoped<GetBatchHandler>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapCreateUpload();
app.MapGetBatch();
app.Run();
