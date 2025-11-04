using AutoMapper;
using CourseApp.EntityLayer.Dto.ExamResultDto;
using CourseApp.EntityLayer.Entity;

namespace CourseApp.ServiceLayer.Mapping;

public class ExamResultMapping : Profile
{
    public ExamResultMapping()
    {
        CreateMap<ExamResult, GetAllExamResultDto>().ReverseMap();
        CreateMap<ExamResult, GetByIdExamResultDto>().ReverseMap();
        CreateMap<ExamResult, CreateExamResultDto>().ReverseMap();
        CreateMap<ExamResult, UpdateExamResultDto>().ReverseMap();
        CreateMap<ExamResult, DeleteExamResultDto>().ReverseMap();

        CreateMap<ExamResult, GetAllExamResultDetailDto>()
            .ForMember(dst => dst.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.Name : string.Empty))
            .ForMember(dst => dst.StudentSurname, opt => opt.MapFrom(src => src.Student != null ? src.Student.Surname : string.Empty))
            .ForMember(dst => dst.ExamName, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Name : string.Empty))
            .ReverseMap();

        CreateMap<ExamResult, GetByIdExamResultDetailDto>()
            .ForMember(dst => dst.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.Name : string.Empty))
            .ForMember(dst => dst.StudentSurname, opt => opt.MapFrom(src => src.Student != null ? src.Student.Surname : string.Empty))
            .ForMember(dst => dst.ExamName, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Name : string.Empty))
            .ReverseMap();
    }
}
