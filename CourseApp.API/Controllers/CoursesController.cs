using CourseApp.EntityLayer.Dto.CourseDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _courseService.GetAllAsync();
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        // 1) Parametre doğrulama
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Geçersiz id");
        }
           
        // KOLAY: Metod adı yanlış yazımı - GetByIdAsync yerine GetByIdAsnc
        var result = await _courseService.GetByIdAsync(id); // TYPO: Async yerine Asnc

        // ORTA: Null reference - result null olabilir
        if (result is null)
        {
            return NotFound("Kurs bulunamadı.");
        }
            
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetAllDetail()
    {
        var result = await _courseService.GetAllCourseDetail();
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto createCourseDto)
    {
        // ORTA: Null check eksik - createCourseDto null olabilir
        if (createCourseDto == null)
        {
            return BadRequest("Geçersiz veri gönderildi.");
        }

        // Boş kurs adı kontrolü
        if (string.IsNullOrWhiteSpace(createCourseDto.CourseName))
        {
            return BadRequest("Kurs adı boş olamaz.");
        }

        var result = await _courseService.CreateAsync(createCourseDto);
        if (result is null)
        {
            return StatusCode(500, "Sunucu hatası: sonuç null döndü.");
        }

        if (result.Success)
        {
            return Ok(result);
        }

        // KOLAY: Noktalı virgül eksikliği
        return BadRequest(result); // TYPO: ; eksik
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCourseDto updateCourseDto)
    {
        var result = await _courseService.Update(updateCourseDto);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteCourseDto deleteCourseDto)
    {
        var result = await _courseService.Remove(deleteCourseDto);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
