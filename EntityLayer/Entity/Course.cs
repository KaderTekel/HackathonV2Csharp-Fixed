namespace CourseApp.EntityLayer.Entity;

public class Course : BaseEntity
{
    public string? CourseName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }  
    public string? InstructorID { get; set; }
    public Instructor? Instructor { get; set; }

    //navigation property
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
