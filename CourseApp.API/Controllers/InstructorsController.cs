using CourseApp.EntityLayer.Dto.InstructorDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _instructorService;

    public InstructorsController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _instructorService.GetAllAsync();

        // Null kontrolü
        if (result == null)
        {
            return StatusCode(500, "Sunucu hatası: servis null döndü.");
        }

        // Servis başarısızsa
        if (!result.Success)
        {
            return BadRequest(result.Message ?? "Eğitmen listesi getirilemedi.");
        }

        // Veri boşsa
        if (result.Data == null || !result.Data.Any())
        {
            return NotFound("Hiç eğitmen bulunamadı.");
        }

        return Ok(result);

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Geçersiz ID değeri.");
        }

        var result = await _instructorService.GetByIdAsync(id);

        if (result == null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }

        if (!result.Success || result.Data == null)
        {
            return NotFound("Eğitmen bulunamadı.");
        }
      
        return Ok(result);

    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatedInstructorDto createdInstructorDto)
    {
        if (createdInstructorDto == null)
        {
            return BadRequest("Geçersiz veri gönderildi.");
        }

        if (string.IsNullOrWhiteSpace(createdInstructorDto.Name))
        {
            return BadRequest("Eğitmen adı boş olamaz.");
        }

        var result = await _instructorService.CreateAsync(createdInstructorDto);

        if (result == null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok(result);

    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatedInstructorDto updatedInstructorDto)
    {
        if (updatedInstructorDto == null)
        {
            return BadRequest("Geçersiz veri gönderildi.");
        }

        if (string.IsNullOrWhiteSpace(updatedInstructorDto.Name))
        {
            return BadRequest("Eğitmen adı boş olamaz.");
        }

        var result = await _instructorService.Update(updatedInstructorDto);

        if (result == null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok(result);

    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeletedInstructorDto deletedInstructorDto)
    {
        if (deletedInstructorDto == null)
        {
            return BadRequest("Geçersiz veri gönderildi.");
        }

        if (string.IsNullOrWhiteSpace(deletedInstructorDto.Id))
        {
            return BadRequest("Geçersiz eğitmen ID değeri.");
        }

        var result = await _instructorService.Remove(deletedInstructorDto);

        if (result == null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return NoContent();
    }
}
