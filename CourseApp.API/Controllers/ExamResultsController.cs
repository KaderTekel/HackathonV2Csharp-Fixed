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
        var result = await _examResultService.GetByIdAsync(id);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetAllDetail()
    {
        var result = await _examResultService.GetAllExamResultDetailAsync();
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetByIdDetail(string id)
    {
        var result = await _examResultService.GetByIdExamResultDetailAsync(id);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExamResultDto createExamResultDto)
    {
        var result = await _examResultService.CreateAsync(createExamResultDto);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateExamResultDto updateExamResultDto)
    {
        var result = await _examResultService.Update(updateExamResultDto);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteExamResultDto deleteExamResultDto)
    {
        var result = await _examResultService.Remove(deleteExamResultDto);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
