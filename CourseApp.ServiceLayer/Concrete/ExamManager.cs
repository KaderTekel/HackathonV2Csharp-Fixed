using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class ExamManager : IExamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllExamDto>>> GetAllAsync(bool track = true)
    {
        // ZOR: Async/await anti-pattern - async metot içinde senkron ToList kullanımı
        var examList = _unitOfWork.Exams.GetAll(track).ToListAsync(); // ZOR: ToListAsync kullanılmalıydı
        // KOLAY: Değişken adı typo - examListMapping yerine examListMapping
        var examListMapping = _mapper.Map<IEnumerable<GetAllExamDto>>(examList); // TYPO
        
        // ORTA: Index out of range - examListMapping boş olabilir
        // IndexOutOfRangeException riski
        
        return new SuccessDataResult<IEnumerable<GetAllExamDto>>(examListMapping, ConstantsMessages.ExamListSuccessMessage);
    }
    public async Task<IDataResult<GetByIdExamDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new ErrorDataResult<GetByIdExamDto>(null, "Geçersiz sınav ID değeri.");

        var hasExam = await _unitOfWork.Exams.GetByIdAsync(id, track);

        if (hasExam == null)
            return new ErrorDataResult<GetByIdExamDto>(null, "Belirtilen ID'ye ait sınav bulunamadı.");

        var examResultMapping = _mapper.Map<GetByIdExamDto>(hasExam);

        return new SuccessDataResult<GetByIdExamDto>(examResultMapping, ConstantsMessages.ExamGetByIdSuccessMessage);
    }
    public async Task<IResult> CreateAsync(CreateExamDto entity)
    {
        if (entity == null)
            return new ErrorResult("Geçersiz veri gönderildi.");

        var addedExamMapping = _mapper.Map<Exam>(entity);

        if (addedExamMapping == null)
            return new ErrorResult("Sınav verisi oluşturulamadı.");

        if (string.IsNullOrWhiteSpace(addedExamMapping.Name))
            return new ErrorResult("Sınav adı boş olamaz.");

        await _unitOfWork.Exams.CreateAsync(addedExamMapping);

        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
            return new SuccessResult(ConstantsMessages.ExamCreateSuccessMessage);

        return new ErrorResult(ConstantsMessages.ExamCreateFailedMessage);
    }


    public async Task<IResult> Remove(DeleteExamDto entity)
    {
        if (entity == null)
            return new ErrorResult("Geçersiz veri gönderildi.");

        if (string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz sınav ID değeri.");

        var existingExam = await _unitOfWork.Exams.GetByIdAsync(entity.Id);
        if (existingExam == null)
            return new ErrorResult("Silinecek sınav bulunamadı.");

        _unitOfWork.Exams.Remove(existingExam);

        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
            return new SuccessResult(ConstantsMessages.ExamDeleteSuccessMessage);

        return new ErrorResult(ConstantsMessages.ExamDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateExamDto entity)
    {
        if (entity == null)
            return new ErrorResult("Geçersiz veri gönderildi.");

        if (string.IsNullOrWhiteSpace(entity.Id))
            return new ErrorResult("Geçersiz sınav ID değeri.");

        var existingExam = await _unitOfWork.Exams.GetByIdAsync(entity.Id);
        if (existingExam == null)
            return new ErrorResult("Güncellenecek sınav bulunamadı.");

        existingExam.Name = entity.Name;
        existingExam.Date = entity.Date;

        _unitOfWork.Exams.Update(existingExam);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
            return new SuccessResult(ConstantsMessages.ExamUpdateSuccessMessage);

        return new ErrorResult(ConstantsMessages.ExamUpdateFailedMessage);
    }
}
