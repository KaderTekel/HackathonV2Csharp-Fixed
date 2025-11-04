using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.StudentDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class StudentManager : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StudentManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllStudentDto>>> GetAllAsync(bool track = true)
    {
        var list = await _unitOfWork.Students.GetAll(track).ToListAsync();
        if (list == null || !list.Any())
            return new ErrorDataResult<IEnumerable<GetAllStudentDto>>(null, ConstantsMessages.StudentListFailedMessage);

        var mapped = _mapper.Map<IEnumerable<GetAllStudentDto>>(list);
        return new SuccessDataResult<IEnumerable<GetAllStudentDto>>(mapped, ConstantsMessages.StudentListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdStudentDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new ErrorDataResult<GetByIdStudentDto>(null, "Geçersiz öğrenci ID değeri.");

        var entity = await _unitOfWork.Students.GetByIdAsync(id, track);
        if (entity == null)
            return new ErrorDataResult<GetByIdStudentDto>(null, "Öğrenci bulunamadı.");

        var mapped = _mapper.Map<GetByIdStudentDto>(entity);
        return new SuccessDataResult<GetByIdStudentDto>(mapped, ConstantsMessages.StudentGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateStudentDto entity)
    {
        if (entity == null)
            return new ErrorResult("Geçersiz veri gönderildi.");

        var mapped = _mapper.Map<Student>(entity);
        await _unitOfWork.Students.CreateAsync(mapped);

        var result = await _unitOfWork.CommitAsync();
        return result > 0
            ? new SuccessResult(ConstantsMessages.StudentCreateSuccessMessage)
            : new ErrorResult(ConstantsMessages.StudentCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteStudentDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz silme verisi.");

        var existing = await _unitOfWork.Students.GetByIdAsync(entity.Id);
        if (existing == null)
            return new ErrorResult("Silinecek öğrenci bulunamadı.");

        _unitOfWork.Students.Remove(existing);
        var result = await _unitOfWork.CommitAsync();

        return result > 0
            ? new SuccessResult(ConstantsMessages.StudentDeleteSuccessMessage)
            : new ErrorResult(ConstantsMessages.StudentDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateStudentDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz güncelleme verisi.");

        var existing = await _unitOfWork.Students.GetByIdAsync(entity.Id);
        if (existing == null)
            return new ErrorResult("Güncellenecek öğrenci bulunamadı.");

        existing.Name = entity.Name;
        existing.Surname = entity.Surname;
        existing.BirthDate = entity.BirthDate;
        existing.TC = entity.TC;

        _unitOfWork.Students.Update(existing);
        var result = await _unitOfWork.CommitAsync();

        return result > 0
            ? new SuccessResult(ConstantsMessages.StudentUpdateSuccessMessage)
            : new ErrorResult(ConstantsMessages.StudentUpdateFailedMessage);
    }

}
