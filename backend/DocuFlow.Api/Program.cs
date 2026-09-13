using DocuFlow.Api.Configuration;
using DocuFlow.Api.Features.Documents;
using DocuFlow.Api.Features.Uploads;
using DocuFlow.Api.Services;
using DocuFlow.Aws.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AwsOptions>(builder.Configuration.GetSection("Aws"));
builder.Services.AddDocuFlowAws(builder.Configuration);
builder.Services.AddSingleton<CreateUploadValidator>();
builder.Services.AddScoped<PresignedUrlService>();
builder.Services.AddScoped<UploadManifestService>();
builder.Services.AddScoped<CreateUploadHandler>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapCreateUpload();
app.MapGetBatch();
app.Run();
