using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.InstructorDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class InstructorManager : IInstructorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InstructorManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllInstructorDto>>> GetAllAsync(bool track = true)
    {
        var list = await _unitOfWork.Instructors.GetAll(track).ToListAsync();
        if (list == null || !list.Any())
            return new ErrorDataResult<IEnumerable<GetAllInstructorDto>>(null, ConstantsMessages.InstructorListFailedMessage);

        var mapped = _mapper.Map<IEnumerable<GetAllInstructorDto>>(list);
        return new SuccessDataResult<IEnumerable<GetAllInstructorDto>>(mapped, ConstantsMessages.InstructorListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdInstructorDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new ErrorDataResult<GetByIdInstructorDto>(null, "Geçersiz eğitmen ID değeri.");

        var entity = await _unitOfWork.Instructors.GetByIdAsync(id, track);
        if (entity == null)
            return new ErrorDataResult<GetByIdInstructorDto>(null, "Eğitmen bulunamadı.");

        var mapped = _mapper.Map<GetByIdInstructorDto>(entity);
        return new SuccessDataResult<GetByIdInstructorDto>(mapped, ConstantsMessages.InstructorGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreatedInstructorDto entity)
    {
        if (entity == null)
            return new ErrorResult("Geçersiz veri gönderildi.");

        var mapped = _mapper.Map<Instructor>(entity);
        await _unitOfWork.Instructors.CreateAsync(mapped);

        var result = await _unitOfWork.CommitAsync();
        return result > 0
            ? new SuccessResult(ConstantsMessages.InstructorCreateSuccessMessage)
            : new ErrorResult(ConstantsMessages.InstructorCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeletedInstructorDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz silme verisi.");

        var existing = await _unitOfWork.Instructors.GetByIdAsync(entity.Id);
        if (existing == null)
            return new ErrorResult("Silinecek eğitmen bulunamadı.");

        _unitOfWork.Instructors.Remove(existing);
        var result = await _unitOfWork.CommitAsync();

        return result > 0
            ? new SuccessResult(ConstantsMessages.InstructorDeleteSuccessMessage)
            : new ErrorResult(ConstantsMessages.InstructorDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdatedInstructorDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz güncelleme verisi.");

        var existing = await _unitOfWork.Instructors.GetByIdAsync(entity.Id);
        if (existing == null)
            return new ErrorResult("Güncellenecek eğitmen bulunamadı.");

        existing.Name = entity.Name;
        existing.Surname = entity.Surname;
        existing.Email = entity.Email;
        existing.Professions = entity.Professions;
        existing.PhoneNumber = entity.PhoneNumber;

        _unitOfWork.Instructors.Update(existing);
        var result = await _unitOfWork.CommitAsync();

        return result > 0
            ? new SuccessResult(ConstantsMessages.InstructorUpdateSuccessMessage)
            : new ErrorResult(ConstantsMessages.InstructorUpdateFailedMessage);
    }
}
