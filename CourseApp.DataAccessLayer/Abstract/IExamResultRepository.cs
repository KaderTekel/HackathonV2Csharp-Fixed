using CourseApp.EntityLayer.Entity;

namespace CourseApp.DataAccessLayer.Abstract;

public interface IExamResultRepository:IGenericRepository<ExamResult>
{
    IQueryable<ExamResult> GetAllExamResultDetails(bool track = true);
    Task<ExamResult> GetByIdExamResultDetailAsync(string id, bool track = true);
}
