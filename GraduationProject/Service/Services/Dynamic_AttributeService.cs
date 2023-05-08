using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.DynamicAttribute;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Transactions;

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
        public ApiResponse AddNewDynamicAttribute(AddDependencyInstViewModel AddDependencyInstViewModel)
        {
            using (TransactionScope Transaction = new TransactionScope(TransactionScopeOption.Required,
                new TimeSpan(0, 15, 0)))
            {
                Dynamic_Attribute NewDynamicAttribute = _Mapper.Map<Dynamic_Attribute>(AddDependencyInstViewModel);

                Data_Type? DataType = _DbContext.Data_Types
                    .FirstOrDefault(x => x.Id == AddDependencyInstViewModel.DataTypeId);

                if (DataType != null)
                {
                    string DataTypeName = DataType.Name;

                    if (DataTypeName.ToLower() == "String".ToLower())
                    {
                        NewDynamicAttribute.DefaultValue = !string.IsNullOrEmpty(AddDependencyInstViewModel.StringDefaultValue) ?
                            AddDependencyInstViewModel.StringDefaultValue : "";
                    }
                    else if (DataTypeName.ToLower() == "int".ToLower())
                    {
                        NewDynamicAttribute.DefaultValue = AddDependencyInstViewModel.DoubleDefaultValue != null ?
                            AddDependencyInstViewModel.DoubleDefaultValue.ToString() : "0";
                    }
                    else if (DataTypeName.ToLower() == "double".ToLower())
                    {
                        NewDynamicAttribute.DefaultValue = AddDependencyInstViewModel.DoubleDefaultValue != null ?
                            AddDependencyInstViewModel.DoubleDefaultValue.ToString() : "0";
                    }
                    else if (DataTypeName.ToLower() == "boolean".ToLower())
                    {
                        NewDynamicAttribute.DefaultValue = AddDependencyInstViewModel.BooleanDefaultValue != null ?
                            AddDependencyInstViewModel.BooleanDefaultValue.ToString() : "false";
                    }
                    else if (DataTypeName.ToLower() == "datetime".ToLower())
                    {
                        NewDynamicAttribute.DefaultValue = AddDependencyInstViewModel.DateTimeDefaultValue != null ?
                            AddDependencyInstViewModel.DateTimeDefaultValue.ToString() : DateTime.Now.ToString();
                    }
                }

                NewDynamicAttribute.Clinic = _DbContext.Clinics.FirstOrDefault(x => x.Id == NewDynamicAttribute.ClinicId);

                // Validation For Dynamic Attribute Key (Dynamic Attribute Key Can't Be Reapeated For The Same TableName)..
                Dynamic_Attribute? CheckNameInTLIDynamic = _DbContext.Dynamic_Attributes.Include(x => x.Clinic).FirstOrDefault(x =>
                    x.Key.ToLower() == NewDynamicAttribute.Key.ToLower() &&
                    x.Clinic.Name.ToLower() == NewDynamicAttribute.Clinic.Name.ToLower());

                if (CheckNameInTLIDynamic != null)
                    return new ApiResponse(false, $"This Attribute Key: ({NewDynamicAttribute.Key}) is Already Exist " +
                        $"in Clinic: ({NewDynamicAttribute.Clinic.Name}) as a Dynamic Attribute");

                // Validation For Dynamic Attribute Key (Can't Add New Dynamic Attribute Key If It is Already Exist in Atttribute Activated Table (TLIattributeActivated))..
                Static_Attribute? CheckNameInTLIAttribute = _DbContext.Static_Attributes.FirstOrDefault(x =>
                    x.Key.ToLower() == NewDynamicAttribute.Key.ToLower() &&
                    x.Clinic.Name.ToLower() == NewDynamicAttribute.Clinic.Name.ToLower());

                if (CheckNameInTLIAttribute != null)
                    return new ApiResponse(false, $"This Attribute Key ({NewDynamicAttribute.Key}) is Already Exist " +
                        $"in Clinic: ({NewDynamicAttribute.Clinic.Name}) as a Static Attribute");

                _DbContext.Dynamic_Attributes.Add(NewDynamicAttribute);
                _DbContext.SaveChanges();

                if (AddDependencyInstViewModel.Validations != null ? AddDependencyInstViewModel.Validations.Count > 0 : false)
                {
                    foreach (ValidationViewModel GeneralValidation in AddDependencyInstViewModel.Validations)
                    {
                        Validation Validation = new Validation();
                        Validation.DynamicAttributeId = NewDynamicAttribute.Id;
                        Validation.OperationId = GeneralValidation.OperationId;

                        if (DataType.Name.ToLower() == "string")
                            Validation.ValueString = GeneralValidation.OperationValue;

                        else if (DataType.Name.ToLower() == "int" || DataType.Name.ToLower() == "double" || DataType.Name.ToLower() == "float")
                            Validation.ValueDouble = Convert.ToDouble(GeneralValidation.OperationValue);

                        else if (DataType.Name.ToLower() == "boolean")
                            Validation.ValueBoolean = Convert.ToBoolean(GeneralValidation.OperationValue);

                        else if (DataType.Name.ToLower() == "datetime")
                            Validation.ValueDateTime = Convert.ToDateTime(GeneralValidation.OperationValue);

                        _DbContext.Validations.Add(Validation);
                    }
                }

                string ClinicName = string.Empty;

                if (AddDependencyInstViewModel.Dependencies != null ? AddDependencyInstViewModel.Dependencies.Count > 0 : false)
                {
                    ClinicName = NewDynamicAttribute.Clinic.Name;

                    foreach (DependencyViewModel DependencyViewModel in AddDependencyInstViewModel.Dependencies)
                    {
                        Dependency DependencyEntity = new Dependency()
                        {
                            DynamicAttributeId = NewDynamicAttribute.Id,
                            OperationId = DependencyViewModel.OperationId
                        };

                        if (DataType != null)
                        {
                            if (DataType.Name.ToLower() == "string")
                                DependencyEntity.ValueString = DependencyViewModel.ValueString;

                            else if (DataType.Name.ToLower() == "int" || DataType.Name.ToLower() == "double" || DataType.Name.ToLower() == "float")
                                DependencyEntity.ValueDouble = Convert.ToDouble(DependencyViewModel.ValueDouble);

                            else if (DataType.Name.ToLower() == "boolean")
                                DependencyEntity.ValueBoolean = Convert.ToBoolean(DependencyViewModel.ValueBoolean);

                            else if (DataType.Name.ToLower() == "datetime")
                                DependencyEntity.ValueDateTime = Convert.ToDateTime(DependencyViewModel.ValueDateTime);

                            _DbContext.Dependencies.Add(DependencyEntity);
                        }

                        Row NewRow = new Row();
                        _DbContext.Rows.Add(NewRow);

                        foreach (AddRuleViewModel DependencyRule in DependencyViewModel.DependencyRules)
                        {
                            Clinic? DependencyClininEntity = NewDynamicAttribute.Clinic;
                            Rule NewRule = _Mapper.Map<Rule>(DependencyRule);
                            NewRule.ClinicId = NewDynamicAttribute.ClinicId;
                            NewRule.ClinicId = DependencyClininEntity.Id;
                            _DbContext.Rules.Add(NewRule);

                            Row_Rule RowRuleEntity = new Row_Rule();

                            RowRuleEntity.RuleId = NewRule.Id;
                            RowRuleEntity.RowId = NewRow.Id;
                            _DbContext.Row_Rules.Add(RowRuleEntity);
                        }
                    }
                }
            }
            return null;
        }
    }
}
