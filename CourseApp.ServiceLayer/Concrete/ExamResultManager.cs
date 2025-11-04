using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamResultDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CourseApp.ServiceLayer.Concrete;

public class ExamResultManager : IExamResultService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamResultManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllExamResultDto>>> GetAllAsync(bool track = true)
    {
        var examResultList = await _unitOfWork.ExamResults.GetAll(track).ToListAsync();
        if (examResultList == null || !examResultList.Any())
            return new ErrorDataResult<IEnumerable<GetAllExamResultDto>>(null, ConstantsMessages.ExamResultListFailedMessage);

        var mappedList = _mapper.Map<IEnumerable<GetAllExamResultDto>>(examResultList);
        return new SuccessDataResult<IEnumerable<GetAllExamResultDto>>(mappedList, ConstantsMessages.ExamResultListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdExamResultDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new ErrorDataResult<GetByIdExamResultDto>(null, "Geçersiz ID değeri.");

        var hasExamResult = await _unitOfWork.ExamResults.GetByIdAsync(id, track);
        if (hasExamResult == null)
            return new ErrorDataResult<GetByIdExamResultDto>(null, "Sonuç bulunamadı.");

        var mappedResult = _mapper.Map<GetByIdExamResultDto>(hasExamResult);
        return new SuccessDataResult<GetByIdExamResultDto>(mappedResult, ConstantsMessages.ExamResultListSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateExamResultDto entity)
    {
        if (entity == null)
            return new ErrorResult("Geçersiz veri gönderildi.");

        var mapped = _mapper.Map<ExamResult>(entity);
        if (mapped == null)
            return new ErrorResult("Dönüştürme başarısız.");

        await _unitOfWork.ExamResults.CreateAsync(mapped);
        var result = await _unitOfWork.CommitAsync();
        return result > 0
             ? new SuccessResult(ConstantsMessages.ExamResultCreateSuccessMessage)
             : new ErrorResult(ConstantsMessages.ExamResultCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteExamResultDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz sınav sonucu ID değeri.");

        var mapped = _mapper.Map<ExamResult>(entity);
        _unitOfWork.ExamResults.Remove(mapped);
        var result = await _unitOfWork.CommitAsync();

        return result > 0
            ? new SuccessResult(ConstantsMessages.ExamResultDeleteSuccessMessage)
            : new ErrorResult(ConstantsMessages.ExamResultDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateExamResultDto entity)
    {
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz güncelleme verisi.");

        var existing = await _unitOfWork.ExamResults.GetByIdAsync(entity.Id);
        if (existing == null)
            return new ErrorResult("Güncellenecek sonuç bulunamadı.");

        existing.Grade = entity.Grade;
        existing.StudentID = entity.StudentID;
        existing.ExamID = entity.ExamID;

        _unitOfWork.ExamResults.Update(existing);
        var result = await _unitOfWork.CommitAsync();

        return result > 0
            ? new SuccessResult(ConstantsMessages.ExamResultUpdateSuccessMessage)
            : new ErrorResult(ConstantsMessages.ExamResultUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllExamResultDetailDto>>> GetAllExamResultDetailAsync(bool track = true)
    {
        var examResultList = await _unitOfWork.ExamResults
            .GetAllExamResultDetail(track)
            .Include(x => x.Student)
            .Include(x => x.Exam)
            .ToListAsync();

        if (examResultList == null || !examResultList.Any())
            return new ErrorDataResult<IEnumerable<GetAllExamResultDetailDto>>(null, ConstantsMessages.ExamResultListFailedMessage);

        var mappedList = _mapper.Map<IEnumerable<GetAllExamResultDetailDto>>(examResultList);
        return new SuccessDataResult<IEnumerable<GetAllExamResultDetailDto>>(mappedList, ConstantsMessages.ExamResultListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdExamResultDetailDto>> GetByIdExamResultDetailAsync(string id, bool track = true)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new ErrorDataResult<GetByIdExamResultDetailDto>(null, "Geçersiz ID değeri.");

        var detail = await _unitOfWork.ExamResults.GetByIdExamResultDetailAsync(id, track);
        if (detail == null)
            return new ErrorDataResult<GetByIdExamResultDetailDto>(null, "Sınav sonucu detayı bulunamadı.");

        var mapped = _mapper.Map<GetByIdExamResultDetailDto>(detail);
        return new SuccessDataResult<GetByIdExamResultDetailDto>(mapped, "Sınav sonucu detayı başarıyla getirildi.");
    }
}
