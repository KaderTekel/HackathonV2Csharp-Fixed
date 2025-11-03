using CourseApp.EntityLayer.Dto.LessonDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _lessonService.GetAllAsync();

        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
            return BadRequest(result.Message ?? "Ders listesi getirilemedi.");

        if (result.Data == null || !result.Data.Any())
            return NotFound("Hiç ders bulunamadı.");

        return Ok(result);

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Geçersiz ID değeri.");

        var result = await _lessonService.GetByIdAsync(id);

        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success || result.Data == null)
            return NotFound("Ders bulunamadı.");

        return Ok(result);

    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetAllDetail()
    {
        var result = await _lessonService.GetAllLessonDetailAsync();
        if (result?.Success == true)
            return Ok(result);

        return BadRequest(result?.Message ?? "Detaylı ders listesi alınamadı.");

    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetByIdDetail(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Geçersiz ID değeri.");

        var result = await _lessonService.GetByIdLessonDetailAsync(id);
        if (result?.Success == true)
            return Ok(result);

        return NotFound("Ders detayı bulunamadı.");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLessonDto createLessonDto)
    {
        if (createLessonDto == null)
        {
            return BadRequest("Geçersiz veri gönderildi.");
        }

        // Ders adı boş mu?
        if (string.IsNullOrWhiteSpace(createLessonDto.Name))
        {
            return BadRequest("Ders adı boş olamaz.");
        }

        // Artık burada güvenle erişebilirsin
        var firstChar = createLessonDto.Name[0];

        var result = await _lessonService.CreateAsync(createLessonDto);

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
    public async Task<IActionResult> Update([FromBody] UpdateLessonDto updateLessonDto)
    {
        if (updateLessonDto == null)
            return BadRequest("Geçersiz veri gönderildi.");

        if (string.IsNullOrWhiteSpace(updateLessonDto.Id))
            return BadRequest("Geçersiz ders ID değeri.");

        if (string.IsNullOrWhiteSpace(updateLessonDto.Name))
            return BadRequest("Ders adı boş olamaz.");

        var result = await _lessonService.Update(updateLessonDto);

        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result);

    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteLessonDto deleteLessonDto)
    {
        if (deleteLessonDto == null)
            return BadRequest("Geçersiz veri gönderildi.");

        if (string.IsNullOrWhiteSpace(deleteLessonDto.Id))
            return BadRequest("Geçersiz ders ID değeri.");

        var result = await _lessonService.Remove(deleteLessonDto);

        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
            return BadRequest(result.Message);

        return NoContent();
    }
}
