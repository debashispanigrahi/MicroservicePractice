using EmployeeRabbitMq.Api.Services;
using EmployeeRabbitMq.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeRabbitMq.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeeController(RabbitMqProducer rabbitMqProducer) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(
        EmployeeCreatedMessage employee)
    {
        var authorization = Request.Headers.Authorization.ToString();
        
        await rabbitMqProducer.PublishEmployeeCreatedAsync(employee);

        return Accepted(new
        {
            message = "Employee message published successfully.",
            employee
        });
    }
}