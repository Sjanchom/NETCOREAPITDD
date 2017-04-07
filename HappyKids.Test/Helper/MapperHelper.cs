using HappyKids.Test.Controllers;

namespace HappyKids.Test.Helper
{
    class MapperHelper
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
                cfg.CreateMap<Student, StudentDTO>().ReverseMap();
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
