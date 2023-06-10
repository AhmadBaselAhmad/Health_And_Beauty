using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Clinic;
using GraduationProject.DataBase.ViewModels.Service;
using GraduationProject.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GraduationProject.Service.Services
{
    public class MyClinicService : IMyClinicService
    {
        private GraduationProjectDbContext _DbContext;
        private readonly IMapper _Mapper;
        public MyClinicService(GraduationProjectDbContext DbContext, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Mapper = Mapper;
        }
        public ApiResponse GetAllClinicServices(int ClinicId, ComplexFilter Filter)
        {
            List<ServiceViewModel> ClinicServices = _Mapper.Map<List<ServiceViewModel>>(_DbContext.Services
                .Include(x => x.Clinic)
                .Where(x => x.ClinicId == ClinicId && (!string.IsNullOrEmpty(Filter.SearchQuery) ? 
                    x.Name.ToLower().StartsWith(Filter.SearchQuery) : true)).ToList());

            int Count = ClinicServices.Count();

            if (!string.IsNullOrEmpty(Filter.Sort))
            {
                PropertyInfo? SortProperty = typeof(ServiceViewModel).GetProperty(Filter.Sort);

                if (SortProperty != null && Filter.Order == "asc")
                    ClinicServices = ClinicServices.OrderBy(x => SortProperty.GetValue(x)).ToList();

                else if (SortProperty != null && Filter.Order == "desc")
                    ClinicServices = ClinicServices.OrderByDescending(x => SortProperty.GetValue(x)).ToList();

                ClinicServices = ClinicServices.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(ClinicServices, string.Empty, Count);
            }
            else
            {
                ClinicServices = ClinicServices.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(ClinicServices, string.Empty, Count);
            }
        }
    }
}
