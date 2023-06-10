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
        public ApiResponse GetAllClinicServices(ComplexFilter Filter, int? ClinicId)
        {
            List<ServiceViewModel> ClinicServices = _Mapper.Map<List<ServiceViewModel>>(_DbContext.Services
                .Include(x => x.Clinic)
                .Where(x => (ClinicId != null ? x.ClinicId == ClinicId : true) && (!string.IsNullOrEmpty(Filter.SearchQuery) ?
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
        public ApiResponse AddNewClinicService(AddClinicServiceViewModel AddClinicServiceViewModel)
        {
            DataBase.Models.Service NewClinicServiceEntity = _Mapper.Map<DataBase.Models.Service>(AddClinicServiceViewModel);

            bool CheckServiceName = _DbContext.Services
                .Any(x => x.Name.ToLower() == NewClinicServiceEntity.Name.ToLower() &&
                    x.ClinicId == NewClinicServiceEntity.Id);

            if (CheckServiceName)
                return new ApiResponse(false, $"This Service Name: ({AddClinicServiceViewModel.Name}) is Already Exist in This Clinic");

            _DbContext.Services.Add(NewClinicServiceEntity);
            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse GetClinicServiceById(int ClinicServiceId)
        {
            DataBase.Models.Service? ClinicServiceEntity = _DbContext.Services
                .Include(x => x.Clinic)
                .FirstOrDefault(x => x.Id == ClinicServiceId);

            if (ClinicServiceEntity == null)
                return new ApiResponse(false, $"No Service Found With This Id: ({ClinicServiceId})");

            ServiceViewModel ClinicServiceViewModel = _Mapper.Map<ServiceViewModel>(ClinicServiceEntity);

            return new ApiResponse(ClinicServiceViewModel, "Succeed");
        }
        public ApiResponse EditClinicService(EditClinicServiceViewModel EditClinicServiceViewModel)
        {
            bool CheckServiceName = _DbContext.Services
                .Any(x => x.Name.ToLower() == EditClinicServiceViewModel.Name.ToLower() &&
                    x.ClinicId == EditClinicServiceViewModel.Id);

            if (CheckServiceName)
                return new ApiResponse(false, $"This Service Name: ({EditClinicServiceViewModel.Name}) is Already Exist in This Clinic");

            DataBase.Models.Service? ClinicServiceEntity = _DbContext.Services
                .FirstOrDefault(x => x.Id == EditClinicServiceViewModel.Id);

            if (ClinicServiceEntity == null)
                return new ApiResponse(false, $"No Service Found With This Id: ({EditClinicServiceViewModel.Id})");

            ClinicServiceEntity.Name = EditClinicServiceViewModel.Name;
            ClinicServiceEntity.Description = EditClinicServiceViewModel.Description;
            ClinicServiceEntity.Step = EditClinicServiceViewModel.Step;

            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse DeleteClinicService(int ClinicServiceId)
        {
            DataBase.Models.Service? ClinicServiceEntity = _DbContext.Services
                .FirstOrDefault(x => x.Id == ClinicServiceId);

            if (ClinicServiceEntity == null)
                return new ApiResponse(false, $"No Service Found With This Id: ({ClinicServiceId})");

            ClinicServiceEntity.IsDeleted = true;

            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
    }
}
