using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Clinic;
using GraduationProject.Service.Interfaces;
using System.Reflection;

namespace GraduationProject.Service.Services
{
    public class ClinicService: IClinicService
    {
        private GraduationProjectDbContext _DbContext;
        private readonly IMapper _Mapper;
        public ClinicService(GraduationProjectDbContext DbContext, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Mapper = Mapper;
        }
        public ApiResponse GetAllClinics(ComplexFilter Filter)
        {
            List<ClinicViewModel> Clinics = _Mapper.Map<List<ClinicViewModel>>(_DbContext.Clinics
                .Where(x => !string.IsNullOrEmpty(Filter.SearchQuery) ? x.Name.ToLower().StartsWith(Filter.SearchQuery) : true).ToList());

            int Count = Clinics.Count();

            if (!string.IsNullOrEmpty(Filter.Sort))
            {
                PropertyInfo? SortProperty = typeof(Clinic).GetProperty(Filter.Sort);

                if (SortProperty != null && Filter.Order == "asc")
                    Clinics = Clinics.OrderBy(x => SortProperty.GetValue(x)).ToList();

                else if (SortProperty != null && Filter.Order == "desc")
                    Clinics = Clinics.OrderByDescending(x => SortProperty.GetValue(x)).ToList();

                Clinics = Clinics.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Clinics, string.Empty, Count);
            }
            else
            {
                Clinics = Clinics.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Clinics, string.Empty, Count);
            }
        }
    }
}
