using BookTracking.Domain.Interfaces;
using BookTracking.Infrastructure.Data;
using BookTracking.Infrastructure.Repositories;
using BookTracking.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using BookTracking.Application.Interfaces;
using BookTracking.Application.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using BookTracking.API.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();

// Custom Validation Response
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        var response = BookTracking.API.Models.ApiResponse<object>.ValidationError(errors);

        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(BookTracking.API.Mappings.ApiMappingProfile), typeof(BookTracking.Application.Mappings.MappingProfile));

// Application Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Application Services
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Database
builder.Services.AddDbContext<BookTrackingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<BookTrackingDbContext>();
        dbContext.Database.Migrate();
        DbSeeder.Seed(dbContext);
    }
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
