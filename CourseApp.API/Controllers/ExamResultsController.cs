using CourseApp.EntityLayer.Dto.ExamResultDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamResultsController : ControllerBase
{
    private readonly IExamResultService _examResultService;

    public ExamResultsController(IExamResultService examResultService)
    {
        _examResultService = examResultService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {

        var result = await _examResultService.GetAllAsync();

        if (result is null)
        {
            return StatusCode(500, "Sunucu hatası: servis yanıt vermedi.");
        }

        if (!result.Success || result.Data == null || !result.Data.Any())
        {
            return NotFound("Herhangi bir sınav sonucu bulunamadı.");
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
        
        var result = await _examResultService.GetByIdAsync(id);

        if (result is null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }

        if (!result.Success)
        {
            return NotFound($"'{id}' numaralı sınav sonucu bulunamadı.");
        }

        return Ok(result);

    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetAllDetail()
    {
        var result = await _examResultService.GetAllExamResultDetailAsync();

        // Servis null dönerse
        if (result is null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }
           
        // Veri yoksa
        if (!result.Success || result.Data == null || !result.Data.Any())
        {
            return NotFound("Herhangi bir detaylı sınav sonucu bulunamadı.");
        }
           
        return Ok(result);

    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetByIdDetail(string id)
    {
        // Parametre kontrolü
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Geçersiz ID değeri.");
        }

        var result = await _examResultService.GetByIdExamResultDetailAsync(id);

        // Servis null dönerse
        if (result is null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }

        // Kayıt bulunamazsa
        if (!result.Success)
        {
            return NotFound($"'{id}' numaralı sınav detayı bulunamadı.");
        }  

        return Ok(result);

    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExamResultDto createExamResultDto)
    {
        if (createExamResultDto == null)
        {
            return BadRequest("Geçersiz veri gönderildi.");
        }

        var result = await _examResultService.CreateAsync(createExamResultDto);
        if (result is null)
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
    public async Task<IActionResult> Update([FromBody] UpdateExamResultDto updateExamResultDto)
    {
        if (updateExamResultDto == null)
        {
            return BadRequest("Geçersiz veri gönderildi.");
        }

        var result = await _examResultService.Update(updateExamResultDto);

        if (result is null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        } 

        if (!result.Success)
        {
            return NotFound("Güncellenecek sınav sonucu bulunamadı.");
        }

        return Ok(result);

    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteExamResultDto deleteExamResultDto)
    {
        if (deleteExamResultDto == null)
        {
            return BadRequest("Geçersiz veri gönderildi.");
        }

        var result = await _examResultService.Remove(deleteExamResultDto);

        if (result is null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }

        if (!result.Success)
        {
            return NotFound("Silinecek sınav sonucu bulunamadı.");
        }
            
        // 204 - Başarılı silme, içerik yok
        return NoContent();
    }
}
