using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.LessonDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class LessonsManager : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LessonsManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllLessonDto>>> GetAllAsync(bool track = true)
    {
        var lessons = await _unitOfWork.Lessons.GetAll(track).ToListAsync();

        if (lessons == null || !lessons.Any())
            return new ErrorDataResult<IEnumerable<GetAllLessonDto>>(null, ConstantsMessages.LessonListFailedMessage);

        var mapped = _mapper.Map<IEnumerable<GetAllLessonDto>>(lessons);
        return new SuccessDataResult<IEnumerable<GetAllLessonDto>>(mapped, ConstantsMessages.LessonListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdLessonDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new ErrorDataResult<GetByIdLessonDto>(null, "Geçersiz ders ID değeri.");

        var entity = await _unitOfWork.Lessons.GetByIdAsync(id, track);
        if (entity == null)
            return new ErrorDataResult<GetByIdLessonDto>(null, "Ders bulunamadı.");

        var mapped = _mapper.Map<GetByIdLessonDto>(entity);
        return new SuccessDataResult<GetByIdLessonDto>(mapped, ConstantsMessages.LessonGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateLessonDto entity)
    {
        if (entity == null)
            return new ErrorResult("Geçersiz veri gönderildi.");

        var mapped = _mapper.Map<Lesson>(entity);
        await _unitOfWork.Lessons.CreateAsync(mapped);

        var result = await _unitOfWork.CommitAsync();
        return result > 0
            ? new SuccessResult(ConstantsMessages.LessonCreateSuccessMessage)
            : new ErrorResult(ConstantsMessages.LessonCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteLessonDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz silme verisi.");

        var existing = await _unitOfWork.Lessons.GetByIdAsync(entity.Id);
        if (existing == null)
            return new ErrorResult("Silinecek ders bulunamadı.");

        _unitOfWork.Lessons.Remove(existing);
        var result = await _unitOfWork.CommitAsync();

        return result > 0
            ? new SuccessResult(ConstantsMessages.LessonDeleteSuccessMessage)
            : new ErrorResult(ConstantsMessages.LessonDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateLessonDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz güncelleme verisi.");

        var existing = await _unitOfWork.Lessons.GetByIdAsync(entity.Id);
        if (existing == null)
            return new ErrorResult("Güncellenecek ders bulunamadı.");

        existing.Name = entity.Name;
        existing.Date = entity.Date;
        existing.Duration = entity.Duration;
        existing.Content = entity.Content;
        existing.CourseID = entity.CourseID;
        existing.Time = entity.Time;

        _unitOfWork.Lessons.Update(existing);
        var result = await _unitOfWork.CommitAsync();

        return result > 0
            ? new SuccessResult(ConstantsMessages.LessonUpdateSuccessMessage)
            : new ErrorResult(ConstantsMessages.LessonUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllLessonDetailDto>>> GetAllLessonDetailAsync(bool track = true)
    {
        var lessons = await _unitOfWork.Lessons.GetAllLessonDetails(track)
            .Include(x => x.Course)
            .ToListAsync();

        if (lessons == null || !lessons.Any())
            return new ErrorDataResult<IEnumerable<GetAllLessonDetailDto>>(null, ConstantsMessages.LessonListFailedMessage);

        var mapped = _mapper.Map<IEnumerable<GetAllLessonDetailDto>>(lessons);
        return new SuccessDataResult<IEnumerable<GetAllLessonDetailDto>>(mapped, ConstantsMessages.LessonListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdLessonDetailDto>> GetByIdLessonDetailAsync(string id, bool track = true)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new ErrorDataResult<GetByIdLessonDetailDto>(null, "Geçersiz ders ID değeri.");

        var lesson = await _unitOfWork.Lessons.GetByIdLessonDetailsAsync(id, track);
        if (lesson == null)
            return new ErrorDataResult<GetByIdLessonDetailDto>(null, "Ders detayı bulunamadı.");

        var mapped = _mapper.Map<GetByIdLessonDetailDto>(lesson);
        return new SuccessDataResult<GetByIdLessonDetailDto>(mapped, ConstantsMessages.LessonGetByIdSuccessMessage);
    }
}
