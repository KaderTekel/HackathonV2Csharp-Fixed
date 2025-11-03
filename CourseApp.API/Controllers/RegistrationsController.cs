using CourseApp.EntityLayer.Dto.RegistrationDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public RegistrationsController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _registrationService.GetAllAsync();

        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
            return BadRequest(result.Message ?? "Kayıt listesi alınamadı.");

        if (result.Data == null || !result.Data.Any())
            return NotFound("Hiç kayıt bulunamadı.");

        return Ok(result);

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Geçersiz ID değeri.");

        var result = await _registrationService.GetByIdAsync(id);

        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success || result.Data == null)
            return NotFound("Kayıt bulunamadı.");

        return Ok(result);

    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetAllDetail()
    {
        var result = await _registrationService.GetAllRegistrationDetailAsync();

        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
            return BadRequest(result.Message ?? "Kayıt detayları alınamadı.");

        if (result.Data == null || !result.Data.Any())
            return NotFound("Kayıt detayı bulunamadı.");

        return Ok(result);

    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetByIdDetail(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Geçersiz ID değeri.");

        var result = await _registrationService.GetByIdRegistrationDetailAsync(id);

        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success || result.Data == null)
            return NotFound("Kayıt detayı bulunamadı.");

        return Ok(result);

    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRegistrationDto createRegistrationDto)
    {
        if (createRegistrationDto == null)
            return BadRequest("Geçersiz kayıt verisi gönderildi.");

        if (createRegistrationDto.Price <= 0)
            return BadRequest("Geçerli bir fiyat değeri girilmelidir.");

        var result = await _registrationService.CreateAsync(createRegistrationDto);

        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
            return BadRequest(result.Message ?? "Kayıt oluşturulamadı.");

        return CreatedAtAction(nameof(GetById), new { id = createRegistrationDto.StudentID }, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatedRegistrationDto updatedRegistrationDto)
    {
        if (updatedRegistrationDto == null)
            return BadRequest("Geçersiz veri gönderildi.");

        if (string.IsNullOrWhiteSpace(updatedRegistrationDto.Id))
            return BadRequest("Geçersiz kayıt ID değeri.");

        var result = await _registrationService.Update(updatedRegistrationDto);

        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
            return BadRequest(result.Message ?? "Kayıt güncellenemedi.");

        return Ok(result);

    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteRegistrationDto deleteRegistrationDto)
    {
        if (deleteRegistrationDto == null)
            return BadRequest("Geçersiz veri gönderildi.");

        if (string.IsNullOrWhiteSpace(deleteRegistrationDto.Id))
            return BadRequest("Geçersiz kayıt ID değeri.");

        var result = await _registrationService.Remove(deleteRegistrationDto);

        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
            return BadRequest(result.Message ?? "Kayıt silinemedi.");

        return NoContent();
    }
}
