using HappyKids.Helper;
using HappyKids.Models.DataTranferObjects;
using HappyKids.Models.Domain;

namespace HappyKids.Configurations
{
    public class MapperHelper
    {
        public static void SetUpMapper()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                //cfg.CreateMap<Entities.Author, Models.AuthorDto>()
                //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                //    $"{src.FirstName} {src.LastName}"))
                //    .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                //    src.DateOfBirth.GetCurrentAge(src.DateOfDeath)));
                cfg.CreateMap<Student, StudentDTO>()
                    .ForMember(desc => desc.BirthDate,
                        opt => opt.MapFrom(src => src.BirthDate.Value.ToString("dd/MM/yyy")))
                    .ReverseMap()
                    .ForMember(desc => desc.BirthDate, opt => opt.MapFrom(src => UtilHelper.PareDateTime(src.BirthDate)));
                cfg.CreateMap<Student, StudentForUpdateDTO>();
                cfg.CreateMap<Student, StudentForCreateDTO>();
                cfg.CreateMap<StudentForManipulationDTO, Student>()
                    .ForMember(desc => desc.BirthDate,
                    opt => opt.MapFrom(src => UtilHelper.PareDateTime(src.BirthDate)));
                //cfg.CreateMap<StudentForCreateDTO, Student>()
                //    .ForMember(desc => desc.BirthDate, 
                //    opt => opt.MapFrom(src =>UtilHelper.PareDateTime(src.BirthDate) ));
                //cfg.CreateMap<StudentForUpdateDTO, Student>()
                //      .ForMember(desc => desc.BirthDate,
                //        opt => opt.MapFrom(src => UtilHelper.PareDateTime(src.BirthDate)));
                //cfg.CreateMap<Entities.Book, Models.BookDto>();

                //cfg.CreateMap<Models.AuthorForCreationDto, Entities.Author>();

                //cfg.CreateMap<Models.AuthorForCreationWithDateOfDeathDto, Entities.Author>();

                //cfg.CreateMap<Models.BookForCreationDto, Entities.Book>();

                //cfg.CreateMap<Models.BookForUpdateDto, Entities.Book>();

                //cfg.CreateMap<Entities.Book, Models.BookForUpdateDto>();
            });

        }
    }
}
