using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Secretary;
using GraduationProject.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GraduationProject.Service.Services
{
    public class AttributeViewManagement : IAttributeViewManagement
    {
        private GraduationProjectDbContext _DbContext;
        private readonly IMapper _Mapper;
        public AttributeViewManagement(GraduationProjectDbContext DbContext, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Mapper = Mapper;
        }
        public ApiResponse GetAllAttributeViewManagement(int ClinicId, ComplexFilter Filter)
        {
            List<AttributeViewManagement> AttributeViewManagment = _Mapper.Map<List<AttributeViewManagement>>(_DbContext.Attributes_View_Management
                .Include(x => x.StaticAttribute).Include(x => x.DynamicAttribute)
                .Where(x => x.ClinicId == ClinicId && 
                    (x.StaticAttributeId != null ? x.StaticAttribute.Enable : false) && 
                    (x.DynamicAttributeId != null ? !x.DynamicAttribute.Disable : false)).ToList());

            int Count = AttributeViewManagment.Count();

            if (!string.IsNullOrEmpty(Filter.Sort))
            {
                PropertyInfo? SortProperty = typeof(AttributeViewManagement).GetProperty(Filter.Sort);

                if (SortProperty != null && Filter.Order == "asc")
                    AttributeViewManagment = AttributeViewManagment.OrderBy(x => SortProperty.GetValue(x)).ToList();

                else if (SortProperty != null && Filter.Order == "desc")
                    AttributeViewManagment = AttributeViewManagment.OrderByDescending(x => SortProperty.GetValue(x)).ToList();

                AttributeViewManagment = AttributeViewManagment.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(AttributeViewManagment, "Succeed", Count);
            }
            else
            {
                AttributeViewManagment = AttributeViewManagment.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(AttributeViewManagment, "Succeed", Count);
            }
        }
        public ApiResponse UpdateAttributeStatus(int AttributeViewManagementId)
        {
            Attribute_View_Management? Attribute_View_Management = _DbContext.Attributes_View_Management
                .FirstOrDefault(x => x.Id == AttributeViewManagementId);

            if (Attribute_View_Management == null)
                return new ApiResponse(false, $"No Attribute's View Found With This Id: ({AttributeViewManagementId})");

            Attribute_View_Management.Enable = !Attribute_View_Management.Enable;
            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
    }
}
