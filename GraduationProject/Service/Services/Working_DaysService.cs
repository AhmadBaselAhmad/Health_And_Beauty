using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Patient;
using GraduationProject.DataBase.ViewModels.Working_Days;
using GraduationProject.Service.Interfaces;
using System.Reflection;

namespace GraduationProject.Service.Services
{
    public class Working_DaysService : IWorking_DaysService
    {
        private GraduationProjectDbContext _DbContext;
        private readonly IMapper _Mapper;
        public Working_DaysService(GraduationProjectDbContext DbContext, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Mapper = Mapper;
        }
        public ApiResponse GetAllWorkingDays(ComplexFilter Filter)
        {
            List<Working_DaysViewModel> Working_DaysViewModel = _Mapper.Map<List<Working_DaysViewModel>>
                (_DbContext.Working_Days.ToList());

            int Count = Working_DaysViewModel.Count();

            if (!string.IsNullOrEmpty(Filter.Sort))
            {
                PropertyInfo? SortProperty = typeof(Working_DaysViewModel).GetProperty(Filter.Sort);

                if (SortProperty != null && Filter.Order == "asc")
                    Working_DaysViewModel = Working_DaysViewModel.OrderBy(x => SortProperty.GetValue(x)).ToList();

                else if (SortProperty != null && Filter.Order == "desc")
                    Working_DaysViewModel = Working_DaysViewModel.OrderByDescending(x => SortProperty.GetValue(x)).ToList();

                Working_DaysViewModel = Working_DaysViewModel.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Working_DaysViewModel, "Succeed", Count);
            }
            else
            {
                Working_DaysViewModel = Working_DaysViewModel.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Working_DaysViewModel, "Succeed", Count);
            }
        }
        public ApiResponse UpdateWorking_DayStatus(int Working_DayId)
        {
            Working_Day? Working_DayEntity = _DbContext.Working_Days
                .FirstOrDefault(x => x.Id == Working_DayId);

            if (Working_DayEntity == null)
                return new ApiResponse(false, $"No Working Day Found With This Id: ({Working_DayId})");

            Working_DayEntity.Off = !Working_DayEntity.Off;

            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
    }
}
