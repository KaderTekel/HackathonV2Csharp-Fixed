using CourseApp.EntityLayer.Dto.StudentDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json; // KOLAY: Eksik using - System.Text.Json kullanılıyor ama using yok
// ZOR: Katman ihlali - Controller'dan direkt DataAccessLayer'a erişim

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    // ZOR: Katman ihlali - Presentation katmanından direkt DataAccess katmanına erişim
    
    // ORTA: Değişken tanımlandı ama asla kullanılmadı ve null olabilir.

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
         // ZOR: Katman ihlali
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // ORTA: Null reference exception riski - _cachedStudents null
         // Mantıksal hata: cache kontrolü yanlış
        
        
        var result = await _studentService.GetAllAsync();
        // KOLAY: Metod adı yanlış yazımı - Success yerine Succes
        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
            return BadRequest(result.Message ?? "Öğrenci listesi getirilemedi.");

        if (result.Data == null || !result.Data.Any())
            return NotFound("Hiç öğrenci bulunamadı.");

        return Ok(result);

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        // ORTA: Null check eksik - id null/empty olabilir
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Geçersiz ID değeri.");
        // ORTA: Index out of range riski - string.Length kullanımı yanlış olabilir
        // ORTA: id 10 karakterden kısa olursa IndexOutOfRangeException
        
        var result = await _studentService.GetByIdAsync(id);
        // ORTA: Null reference exception - result.Data null olabilir
        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü."); // Null check yok

        if (!result.Success || result.Data == null)
            return NotFound("Öğrenci bulunamadı.");

        return Ok(result);

    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto createStudentDto)
    {
        // ORTA: Null check eksik
        if (createStudentDto == null)
            return BadRequest("Geçersiz veri gönderildi.");

        if (string.IsNullOrWhiteSpace(createStudentDto.Name))
            return BadRequest("Öğrenci adı boş olamaz.");



        // ORTA: Tip dönüşüm hatası - string'i int'e direkt atama
        // ORTA: InvalidCastException - string int'e dönüştürülemez

        // ZOR: Katman ihlali - Controller'dan direkt DbContext'e erişim (Business Logic'i bypass ediyor)
        var result = await _studentService.CreateAsync(createStudentDto);

        if(result == null)
        return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);

    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateStudentDto updateStudentDto)
    {
        if (updateStudentDto == null)
            return BadRequest("Geçersiz veri gönderildi.");

        if (string.IsNullOrWhiteSpace(updateStudentDto.Name))
            return BadRequest("Öğrenci adı boş olamaz.");

        // KOLAY: Değişken adı typo - updateStudentDto yerine updateStudntDto
        // TYPO
        
        var result = await _studentService.Update(updateStudentDto);
        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result);

    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteStudentDto deleteStudentDto)
    {
        // ORTA: Null reference - deleteStudentDto null olabilir
        if (deleteStudentDto == null)
            return BadRequest("Geçersiz veri gönderildi."); // Null check yok

        if (string.IsNullOrWhiteSpace(deleteStudentDto.Id))
            return BadRequest("Geçersiz öğrenci ID değeri.");


        // ZOR: Memory leak - DbContext Dispose edilmiyor
        // Dispose edilmeden kullanılıyor
        
        var result = await _studentService.Remove(deleteStudentDto);
        if (result == null)
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");

        if (!result.Success)
            return BadRequest(result.Message ?? "Silme işlemi başarısız.");

        return NoContent();
    }
}
