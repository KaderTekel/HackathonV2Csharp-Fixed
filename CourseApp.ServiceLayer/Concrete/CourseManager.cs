using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.CourseDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace CourseApp.ServiceLayer.Concrete;

public class CourseManager : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IDataResult<IEnumerable<GetAllCourseDto>>> GetAllAsync(bool track = true)
    {
        // ZOR: N+1 Problemi - Her course için Instructor ayrı sorgu ile çekiliyor
        var courseList = await _unitOfWork.Courses.GetAll(track).Include(x => x.Instructor).ToListAsync();

        if (courseList == null || !courseList.Any())
            return new ErrorDataResult<IEnumerable<GetAllCourseDto>>(null, "Hiç kurs bulunamadı.");

        // ZOR: N+1 - Include/ThenInclude kullanılmamış, lazy loading aktif
        var result = courseList.Select(course => new GetAllCourseDto
        {
            Id = course.ID,
            CourseName = course.CourseName,
            CreatedDate = course.CreatedDate,
            EndDate = course.EndDate,
            InstructorID = course.InstructorID,
            InstructorName = course.Instructor?.Name ?? "Bilinmiyor",
            // ZOR: Her course için ayrı sorgu - course.Instructor?.Name çekiliyor
            // ORTA: Null reference riski - course null olabilir
            IsActive = course.IsActive,
            StartDate = course.StartDate
        }).ToList();

        // ORTA: Index out of range - result boş olabilir
        // IndexOutOfRangeException riski

        return new SuccessDataResult<IEnumerable<GetAllCourseDto>>(result, ConstantsMessages.CourseListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdCourseDto>> GetByIdAsync(string id, bool track = true)
    {
        // ORTA: Null check eksik - id null/empty olabilir
        if (string.IsNullOrWhiteSpace(id))
            return new ErrorDataResult<GetByIdCourseDto>(null, "Geçersiz ID değeri.");

        // ORTA: Null reference exception - hasCourse null olabilir ama kontrol edilmiyor
        var hasCourse = await _unitOfWork.Courses.GetByIdAsync(id, track);
        if (hasCourse == null)
            return new ErrorDataResult<GetByIdCourseDto>(null, "Kurs bulunamadı.");

        // ORTA: Null reference - hasCourse null ise NullReferenceException
        var course = new GetByIdCourseDto
        {
            Id = hasCourse.ID,
            CourseName = hasCourse.CourseName,
            CreatedDate = hasCourse.CreatedDate,
            EndDate = hasCourse.EndDate,
            InstructorID = hasCourse.InstructorID,
            IsActive = hasCourse.IsActive,
            StartDate = hasCourse.StartDate
        };

        return new SuccessDataResult<GetByIdCourseDto>(course, ConstantsMessages.CourseGetByIdSuccessMessage);
    }
    public async Task<IResult> CreateAsync(CreateCourseDto entity)
    {
        if (entity == null)
            return new ErrorResult("Geçersiz veri gönderildi.");

        if (string.IsNullOrWhiteSpace(entity.CourseName))
            return new ErrorResult("Kurs adı boş olamaz.");

        if (entity.EndDate <= entity.StartDate)
            return new ErrorResult("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

        var createdCourse = new Course
        {
            CourseName = entity.CourseName,
            CreatedDate = entity.CreatedDate,
            EndDate = entity.EndDate,
            InstructorID = entity.InstructorID,
            IsActive = entity.IsActive,
            StartDate = entity.StartDate,
        };

        await _unitOfWork.Courses.CreateAsync(createdCourse);

        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.CourseCreateSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.CourseCreateFailedMessage);
    }
    public async Task<IResult> Remove(DeleteCourseDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz veri gönderildi.");

        var existingCourse = await _unitOfWork.Courses.GetByIdAsync(entity.Id);
        if (existingCourse == null)
            return new ErrorResult("Silinmek istenen kurs bulunamadı.");

        _unitOfWork.Courses.Remove(existingCourse);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.CourseDeleteSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.CourseDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateCourseDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz veri gönderildi.");

        if (string.IsNullOrWhiteSpace(entity.CourseName))
            return new ErrorResult("Kurs adı boş olamaz.");

        if (entity.EndDate <= entity.StartDate)
            return new ErrorResult("Bitiş tarihi, başlangıç tarihinden sonra olmalıdır.");

        var updatedCourse = await _unitOfWork.Courses.GetByIdAsync(entity.Id);
        if (updatedCourse == null)
        {
            return new ErrorResult(ConstantsMessages.CourseUpdateFailedMessage);
        }

        updatedCourse.CourseName = entity.CourseName;
        updatedCourse.StartDate = entity.StartDate;
        updatedCourse.EndDate = entity.EndDate;
        updatedCourse.InstructorID = entity.InstructorID;
        updatedCourse.IsActive = entity.IsActive;

        _unitOfWork.Courses.Update(updatedCourse);
        var result = await _unitOfWork.CommitAsync();
        return result > 0
        ? new SuccessResult(ConstantsMessages.CourseUpdateSuccessMessage)
        : new ErrorResult(ConstantsMessages.CourseUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllCourseDetailDto>>> GetAllCourseDetail(bool track = true)
    {
        // ZOR: N+1 Problemi - Include kullanılmamış, lazy loading aktif
        var courseListDetailList = await _unitOfWork.Courses.GetAllCourseDetail(track).Include(x => x.Instructor).ToListAsync();

        if (courseListDetailList == null || !courseListDetailList.Any())
            return new ErrorDataResult<IEnumerable<GetAllCourseDetailDto>>(null, "Hiç kurs bulunamadı.");

        // ZOR: N+1 - Her course için Instructor ayrı sorgu ile çekiliyor (x.Instructor?.Name)
        var courseDetailDtoList  = courseListDetailList.Select(x => new GetAllCourseDetailDto // KOLAY: Yanlış tip - GetAllCourseDetailDto olmalıydı
        {
            CourseName = x.CourseName,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            CreatedDate = x.CreatedDate,
            Id = x.ID,
            InstructorID = x.InstructorID,
            // ZOR: N+1 - Her course için ayrı Instructor sorgusu
            InstructorName = x.Instructor?.Name ?? "Eğitmen bilgisi yok", // Lazy loading aktif - her iterasyonda DB sorgusu
            IsActive = x.IsActive,
        });

        // ORTA: Null reference - courseDetailDtoList null olabilir
        // Null/Empty durumunda exception

        return new SuccessDataResult<IEnumerable<GetAllCourseDetailDto>>(courseDetailDtoList, ConstantsMessages.CourseDetailsFetchedSuccessfully);
    }

    private IResult CourseNameIsNullOrEmpty(string courseName)
    {
        if (string.IsNullOrWhiteSpace(courseName))
            return new ErrorResult("Kurs adı boş olamaz.");
        return new SuccessResult();
    }

    private async Task<IResult> CourseNameUniqeCheck(string id,string courseName)
    {
        var exists = await _unitOfWork.Courses.GetAll(false)
        .AnyAsync(c => c.CourseName == courseName && c.ID != id);

        if (exists)
            return new ErrorResult("Bu kurs adıyla zaten bir kurs var.");

        return new SuccessResult();
    }

    private  IResult CourseNameLenghtCheck(string courseName)
    {
        if (string.IsNullOrEmpty(courseName) || courseName.Length < 2 || courseName.Length > 50)
            return new ErrorResult("Kurs adı uzunluğu 2-50 karakter arasında olmalı.");

        return new SuccessResult();
    }

    private IResult IsValidDateFormat(string date)
    {
        if (!DateTime.TryParse(date, out _))
            return new ErrorResult("Geçersiz tarih formatı.");

        return new SuccessResult();
    }
    private IResult CheckCourseDates(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
            return new ErrorResult("Bitiş tarihi, başlangıç tarihinden sonra olmalıdır.");

        return new SuccessResult();
    }
    
    private IResult CheckInstructorNameIsNullOrEmpty(string instructorName)
    {
        if (string.IsNullOrWhiteSpace(instructorName))
            return new ErrorResult("Eğitmen adı boş olamaz.");

        return new SuccessResult();
    }
}
