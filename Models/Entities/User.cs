using System;
using System.ComponentModel.DataAnnotations;
using kalamon_University.Models.Entities; 

namespace kalamon_University.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public  string Email { get; set; }
        [Required]
        public  string Password { get; set; }
        public UserRole Role { get; set; } = UserRole.Student;

        /* الطالب يتم تعريفه في جدول Students وله مفتاح أجنبي يشير إلى UserID.
        الأستاذ كذلك يُخزن في جدول Professors وله مفتاح أجنبي يشير إلى UserID

       User يمكن أن يكون إما مرتبطًا بسجل في Students أو سجل في Professors.

    العلاقة اختيارية وليست إلزامية، لذا استخدمنا ? بعد Student وProfessor.
        */
        public Student? Student { get; set; }
        public Professor? Professor { get; set; }


    }
}
