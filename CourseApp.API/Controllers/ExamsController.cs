using CourseApp.EntityLayer.Dto.ExamDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamsController : ControllerBase
{
    private readonly IExamService _examService;

    public ExamsController(IExamService examService)
    {
        _examService = examService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {

        var result = await _examService.GetAllAsync();

        // Null kontrolü
        if (result == null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        // N+1 problemi kaldırıldı, controller sadece veriyi döner
        if (result.Data == null || !result.Data.Any())
        {
            return NotFound("Kayıt bulunamadı.");
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
          
        var result = await _examService.GetByIdAsync(id);

        if (result == null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }

        if (!result.Success)
        {
            return NotFound(result.Message ?? "Sınav bulunamadı.");
        }

        return Ok(result);

    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExamDto createExamDto)
    {
        if (createExamDto == null)
        {
            return BadRequest("Geçersiz veri gönderildi.");
        }

        var result = await _examService.CreateAsync(createExamDto);

        if (result == null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }
           
        // Başarıyla oluşturuldu
        return Ok(result);

    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateExamDto updateExamDto)
    {
        if (updateExamDto == null)
        {
            return BadRequest("Güncellenecek veri gönderilmedi.");
        }
          
        var result = await _examService.Update(updateExamDto);

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
    public async Task<IActionResult> Delete([FromBody] DeleteExamDto deleteExamDto)
    {
        if (deleteExamDto == null)
        {
            return BadRequest("Silinecek veri gönderilmedi.");
        }

        var result = await _examService.Remove(deleteExamDto);

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
}
