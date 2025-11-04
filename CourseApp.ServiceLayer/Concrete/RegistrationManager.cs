using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.RegistrationDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class RegistrationManager : IRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegistrationManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllRegistrationDto>>> GetAllAsync(bool track = true)
    {
        var list = await _unitOfWork.Registrations.GetAll(track).ToListAsync();
        if (list == null || !list.Any())
            return new ErrorDataResult<IEnumerable<GetAllRegistrationDto>>(null, ConstantsMessages.RegistrationListFailedMessage);

        var mapped = _mapper.Map<IEnumerable<GetAllRegistrationDto>>(list);
        return new SuccessDataResult<IEnumerable<GetAllRegistrationDto>>(mapped, ConstantsMessages.RegistrationListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdRegistrationDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new ErrorDataResult<GetByIdRegistrationDto>(null, "Geçersiz kayıt ID değeri.");

        var entity = await _unitOfWork.Registrations.GetByIdAsync(id, track);
        if (entity == null)
            return new ErrorDataResult<GetByIdRegistrationDto>(null, "Kayıt bulunamadı.");

        var mapped = _mapper.Map<GetByIdRegistrationDto>(entity);
        return new SuccessDataResult<GetByIdRegistrationDto>(mapped, ConstantsMessages.RegistrationGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateRegistrationDto entity)
    {
        if (entity == null)
            return new ErrorResult("Geçersiz veri gönderildi.");

        var mapped = _mapper.Map<Registration>(entity);
        await _unitOfWork.Registrations.CreateAsync(mapped);

        var result = await _unitOfWork.CommitAsync();
        return result > 0
            ? new SuccessResult(ConstantsMessages.RegistrationCreateSuccessMessage)
            : new ErrorResult(ConstantsMessages.RegistrationCreateFailedMessage);
    }

    public async Task<IResult> Update(UpdatedRegistrationDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz güncelleme verisi.");

        var existing = await _unitOfWork.Registrations.GetByIdAsync(entity.Id);
        if (existing == null)
            return new ErrorResult("Güncellenecek kayıt bulunamadı.");

        existing.StudentID = entity.StudentID;
        existing.CourseID = entity.CourseID;
        existing.Price = entity.Price;
        existing.RegistrationDate = entity.RegistrationDate;

        _unitOfWork.Registrations.Update(existing);
        var result = await _unitOfWork.CommitAsync();

        return result > 0
            ? new SuccessResult(ConstantsMessages.RegistrationUpdateSuccessMessage)
            : new ErrorResult(ConstantsMessages.RegistrationUpdateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteRegistrationDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz silme verisi.");

        var existing = await _unitOfWork.Registrations.GetByIdAsync(entity.Id);
        if (existing == null)
            return new ErrorResult("Silinecek kayıt bulunamadı.");

        _unitOfWork.Registrations.Remove(existing);
        var result = await _unitOfWork.CommitAsync();

        return result > 0
            ? new SuccessResult(ConstantsMessages.RegistrationDeleteSuccessMessage)
            : new ErrorResult(ConstantsMessages.RegistrationDeleteFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllRegistrationDetailDto>>> GetAllRegistrationDetailAsync(bool track = true)
    {
        var details = await _unitOfWork.Registrations.GetAllRegistrationDetail(track)
            .Include(x => x.Student)
            .Include(x => x.Course)
            .ToListAsync();

        if (details == null || !details.Any())
            return new ErrorDataResult<IEnumerable<GetAllRegistrationDetailDto>>(null, ConstantsMessages.RegistrationListFailedMessage);

        var mapped = _mapper.Map<IEnumerable<GetAllRegistrationDetailDto>>(details);
        return new SuccessDataResult<IEnumerable<GetAllRegistrationDetailDto>>(mapped, ConstantsMessages.RegistrationListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdRegistrationDetailDto>> GetByIdRegistrationDetailAsync(string id, bool track = true)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new ErrorDataResult<GetByIdRegistrationDetailDto>(null, "Geçersiz kayıt ID değeri.");

        var entity = await _unitOfWork.Registrations.GetByIdAsync(id, track);

        if (entity == null)
            return new ErrorDataResult<GetByIdRegistrationDetailDto>(null, "Kayıt bulunamadı.");

        var mapped = _mapper.Map<GetByIdRegistrationDetailDto>(entity);
        return new SuccessDataResult<GetByIdRegistrationDetailDto>(mapped, ConstantsMessages.RegistrationGetByIdSuccessMessage);
    }
}
