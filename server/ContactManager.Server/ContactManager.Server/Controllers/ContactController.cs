using ContactManager.Server.Data;
using ContactManager.Server.Dtos;
using ContactManager.Server.Models;
using ContactManager.Server.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly ContactService _contactService;

    public ContactController(AppDbContext context)
    {
        _contactService = new ContactService(context);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Ok(await _contactService.GetUserContacts(userId!));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Contact>> GetContact(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var contact = await _contactService.GetUserContact(id, userId!);

        return contact != null ? Ok(contact) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Contact>> CreateContact(
        ContactDto contactDto,
        [FromServices] IValidator<ContactDto> validator)
    {
        var validationResult = await validator.ValidateAsync(contactDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                )
            });
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var contact = await _contactService.CreateContact(contactDto, userId!);

        return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContact(
        Guid id,
        ContactDto contactDto,
        [FromServices] IValidator<ContactDto> validator)
    {
        var validationResult = await validator.ValidateAsync(contactDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                )
            });
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _contactService.UpdateContact(id, contactDto, userId!);

        return result != null ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var success = await _contactService.DeleteContact(id, userId!);

        return success ? NoContent() : NotFound();
    }
}
