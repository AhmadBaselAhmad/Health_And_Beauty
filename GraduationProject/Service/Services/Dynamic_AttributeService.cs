using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.DynamicAttribute;
using GraduationProject.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GraduationProject.Service.Services
{
    public class Dynamic_AttributeService : IDynamic_AttributeService
    {
        private GraduationProjectDbContext _DbContext;
        private readonly IMapper _Mapper;
        public Dynamic_AttributeService(GraduationProjectDbContext DbContext, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Mapper = Mapper;
        }
        public ApiResponse GetDynamicAttributeById(int Dynamic_AttributeId)
        {
            Dynamic_Attribute? DynamicAttribute = _DbContext.Dynamic_Attributes
                .Include(x => x.DataType).Include(x => x.Clinic)
                .FirstOrDefault(x => x.Id == Dynamic_AttributeId);

            if (DynamicAttribute == null)
                return new ApiResponse(false, $"No Dynamic Attribute Found With This Id: {Dynamic_AttributeId}");

            Dynamic_AttributeViewModel Dynamic_AttributeViewModel = _Mapper.Map<Dynamic_AttributeViewModel>(DynamicAttribute);

            return new ApiResponse(Dynamic_AttributeViewModel, "Succeed");
        }
        public ApiResponse GetAllDynamicAttributes(ComplexFilter Filter, bool? OnlyHealthStandards)
        {
            List<Dynamic_AttributeViewModel> DynamicAttributes = _Mapper.Map<List<Dynamic_AttributeViewModel>>(_DbContext.Dynamic_Attributes
                .Include(x => x.DataType).Include(x => x.Clinic)
                .Where(x => (OnlyHealthStandards != null ? x.IsHealthStandard == OnlyHealthStandards : true) &&
                    (!string.IsNullOrEmpty(Filter.SearchQuery) ? x.Key.ToLower().StartsWith(Filter.SearchQuery) : true)).ToList());

            int Count = DynamicAttributes.Count();

            if (!string.IsNullOrEmpty(Filter.Sort))
            {
                PropertyInfo? SortProperty = typeof(Dynamic_AttributeViewModel).GetProperty(Filter.Sort);

                if (SortProperty != null && Filter.Order == "asc")
                    DynamicAttributes = DynamicAttributes.OrderBy(x => SortProperty.GetValue(x)).ToList();

                else if (SortProperty != null && Filter.Order == "desc")
                    DynamicAttributes = DynamicAttributes.OrderByDescending(x => SortProperty.GetValue(x)).ToList();

                DynamicAttributes = DynamicAttributes.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(DynamicAttributes, "Succeed", Count);
            }
            else
            {
                DynamicAttributes = DynamicAttributes.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(DynamicAttributes, "Succeed", Count);
            }
        }
    }
}
