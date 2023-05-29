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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

                        _DbContext.Dependency_Rows.Add(new Dependency_Row()
                        {
                            DependencyId = DependencyEntity.Id,
                            RowId = NewRow.Id
                        });

                        _DbContext.SaveChanges();

                    }
                }

                _DbContext.Attributes_View_Management.Add(new Attribute_View_Management
                {
                    DynamicAttributeId = NewDynamicAttribute.Id,
                    Enable = true,
                    ClinicId = AddDependencyInstViewModel.ClinicId
                });

                _DbContext.SaveChanges();

                // AddLibraryListValues(addDependencyViewModel, DynamicAttId);

                Transaction.Complete();

                return new ApiResponse(true, "Succeed");
            }
        }
        //public void AddListValues(AddDependencyViewModel AddDependencyViewModel, int DynamicAttId)
        //{
        //    try
        //    {
        //        if (AddDependencyViewModel.Dependencies != null ? AddDependencyViewModel.Dependencies.Count() == 0 : true)
        //        {
        //            AddDefaultValues(AddDependencyViewModel, DynamicAttId);
        //        }
        //        else
        //        {
        //            if (AddDependencyViewModel.BooleanResult != null || AddDependencyViewModel.DoubleResult != null ||
        //                AddDependencyViewModel.DateTimeResult != null || !string.IsNullOrEmpty(AddDependencyViewModel.StringResult))
        //            {
        //                // Civils..
        //                if (AddDependencyViewModel.TableName.ToLower() == TablesNames.TLIcivilWithLegLibrary.ToString().ToLower())
        //                {
        //                    List<CivilWithLegLibraryViewModel> CivilWithLegLibraries = Mapper.Map<List<CivilWithLegLibraryViewModel>>(_unitOfWork.CivilWithLegLibraryRepository.GetIncludeWhere(x =>
        //                        x.Id > 0 && !x.Deleted, x => x.civilSteelSupportCategory, x => x.sectionsLegType, x => x.structureType, x => x.supportTypeDesigned).ToList());

        //                    foreach (DependencyViewModel Dependency in AddDependencyViewModel.Dependencies)
        //                    {
        //                        foreach (CivilWithLegLibraryViewModel CivilWithLegLibrary in CivilWithLegLibraries)
        //                        {
        //                            List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

        //                            foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
        //                            {
        //                                int Succeed = 0;

        //                                foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
        //                                {
        //                                    if (RowRule.Rule.attributeActivatedId != null)
        //                                    {
        //                                        TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

        //                                        PropertyInfo LibraryProp = typeof(CivilWithLegLibraryViewModel).GetProperties().FirstOrDefault(x =>
        //                                            x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

        //                                        object PropObject = LibraryProp.GetValue(CivilWithLegLibrary, null);

        //                                        if (PropObject != null)
        //                                        {
        //                                            string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
        //                                            if (OperationStatic == "==")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueBoolean != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueString == PropObject.ToString())
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                            else if (OperationStatic == "!=")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueBoolean != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueString != PropObject.ToString())
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                            else if (OperationStatic == "<")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                            else if (OperationStatic == "<=")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                            else if (OperationStatic == ">")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                            else if (OperationStatic == ">=")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                    else if (RowRule.Rule.dynamicAttId != null)
        //                                    {
        //                                        TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
        //                                            x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == CivilWithLegLibrary.Id);

        //                                        if (RuleDynamicAttribute != null)
        //                                        {
        //                                            string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

        //                                            if (OperationDynamic == "==")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                            else if (OperationDynamic == "!=")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                            else if (OperationDynamic == ">")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                            else if (OperationDynamic == ">=")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                            else if (OperationDynamic == "<")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                            else if (OperationDynamic == "<=")
        //                                            {
        //                                                if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                                else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
        //                                                {
        //                                                    if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
        //                                                    {
        //                                                        Succeed++;
        //                                                    }
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                                if (Succeed == DependencyRow.RowRules.Count())
        //                                {
        //                                    TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
        //                                        !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == AddDependencyViewModel.tablesNamesId &&
        //                                        x.InventoryId == CivilWithLegLibrary.Id);

        //                                    if (Check == null)
        //                                    {
        //                                        ListToAdd.Add(new TLIdynamicAttLibValue
        //                                        {
        //                                            disable = false,
        //                                            DynamicAttId = DynamicAttId,
        //                                            InventoryId = CivilWithLegLibrary.Id,
        //                                            tablesNamesId = AddDependencyViewModel.tablesNamesId,
        //                                            ValueBoolean = AddDependencyViewModel.BooleanResult,
        //                                            ValueString = AddDependencyViewModel.StringResult,
        //                                            ValueDateTime = AddDependencyViewModel.DateTimeResult,
        //                                            ValueDouble = AddDependencyViewModel.DoubleResult
        //                                        });
        //                                    }
        //                                }
        //                            }
        //                            if (AddDependencyViewModel.BooleanDefaultValue != null || AddDependencyViewModel.DoubleDefaultValue != null ||
        //                                AddDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(AddDependencyViewModel.StringDefaultValue))
        //                            {
        //                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
        //                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == AddDependencyViewModel.tablesNamesId &&
        //                                    x.InventoryId == CivilWithLegLibrary.Id);

        //                                if (Check == null)
        //                                {
        //                                    ListToAdd.Add(new TLIdynamicAttLibValue
        //                                    {
        //                                        disable = false,
        //                                        DynamicAttId = DynamicAttId,
        //                                        InventoryId = CivilWithLegLibrary.Id,
        //                                        tablesNamesId = AddDependencyViewModel.tablesNamesId,
        //                                        ValueBoolean = AddDependencyViewModel.BooleanDefaultValue,
        //                                        ValueString = AddDependencyViewModel.StringDefaultValue,
        //                                        ValueDateTime = AddDependencyViewModel.DateTimeDefaultValue,
        //                                        ValueDouble = AddDependencyViewModel.DoubleDefaultValue
        //                                    });
        //                                }
        //                            }
        //                            _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
        //                            _unitOfWork.SaveChanges();
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                AddDefaultValues(AddDependencyViewModel, DynamicAttId);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //public void LoopForPath(List<string> Path, int StartIndex, ApplicationDbContext _dbContext, object Value, List<int> OutPutIds)
        //{
        //    if (StartIndex == Path.Count())
        //    {
        //        OutPutIds.Add((int)Value);
        //    }
        //    else
        //    {
        //        List<object> TableRecords = Mapper.Map<List<object>>(_dbContext.GetType()
        //            .GetProperties().FirstOrDefault(x => x.Name.ToLower() == Path[StartIndex].ToLower())
        //            .GetValue(_dbContext, null))
        //                .Where(x => x.GetType().GetProperty(Path[StartIndex + 1]).GetValue(x, null) != null ?
        //                    x.GetType().GetProperty(Path[StartIndex + 1]).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower() : false).ToList();

        //        foreach (object Record in TableRecords)
        //        {
        //            // The New Value..
        //            object PrimaryKeyValue = Record.GetType().GetProperty(Path[StartIndex + 2]).GetValue(Record, null);

        //            if (PrimaryKeyValue != null)
        //            {
        //                if (StartIndex + 3 < Path.Count())
        //                    LoopForPath(Path, StartIndex + 3, _dbContext, PrimaryKeyValue, OutPutIds);

        //                else if (StartIndex + 3 == Path.Count())
        //                    OutPutIds.Add((int)PrimaryKeyValue);
        //            }
        //        }
        //    }
        //}
        //public List<int> GetRecordsIds(string MainTableName, AddInstRuleViewModel Rule)
        //{
        //    string SDTableName = Rule.TableName;
        //    string Operation = _unitOfWork.OperationRepository.GetByID(Rule.OperationId.Value).Name;
        //    object Value = new object();

        //    string DataType = "";
        //    if (Rule.OperationValueBoolean != null)
        //    {
        //        DataType = "Bool";
        //        Value = Rule.OperationValueBoolean;
        //    }
        //    else if (Rule.OperationValueDateTime != null)
        //    {
        //        DataType = "DateTime";
        //        Value = Rule.OperationValueDateTime;
        //    }
        //    else if (Rule.OperationValueDouble != null)
        //    {
        //        DataType = "Double";
        //        Value = Rule.OperationValueDouble;
        //    }
        //    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
        //    {
        //        DataType = "String";
        //        Value = Rule.OperationValueString;
        //    }

        //    List<int> OutPutIds = new List<int>();

        //    PathToAddDynamicAttValue Item = new PathToAddDynamicAttValue();

        //    if (MainTableName.ToLower() == TablesNames.TLIradioRRU.ToString().ToLower() || SDTableName.ToLower() == TablesNames.TLIradioRRU.ToString().ToLower())
        //        Item = (PathToAddDynamicAttValue)Enum.Parse(typeof(PathToAddDynamicAttValue),
        //            (MainTableName + SDTableName).ToLower());

        //    else
        //        Item = (PathToAddDynamicAttValue)Enum.Parse(typeof(PathToAddDynamicAttValue),
        //            MainTableName + SDTableName);

        //    List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

        //    if (Path[0].ToLower() == MainTableName.ToLower() &&
        //        Path[1].ToLower() == SDTableName.ToLower())
        //    {
        //        List<object> TableRecords = new List<object>();
        //        if (Rule.attributeActivatedId != null && !Rule.IsDynamic)
        //        {
        //            string AttributeName = _unitOfWork.AttributeActivatedRepository
        //                .GetByID(Rule.attributeActivatedId.Value).Key;

        //            TableRecords = Mapper.Map<List<object>>(_dbContext.GetType()
        //                .GetProperties().FirstOrDefault(x => x.Name.ToLower() == Path[1].ToLower()).GetValue(_dbContext, null))
        //                    .Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ?
        //                       (Operation == ">" ?
        //                            (DataType.ToLower() == "DateTime".ToLower() ?
        //                                Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 :
        //                            DataType.ToLower() == "Double".ToLower() ?
        //                                Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 : false) :
        //                        Operation == ">=" ?
        //                            (DataType.ToLower() == "DateTime".ToLower() ?
        //                                (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 ||
        //                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) :
        //                            DataType.ToLower() == "Double".ToLower() ?
        //                                (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 ||
        //                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) : false) :
        //                        Operation == "<" ?
        //                            (DataType.ToLower() == "DateTime".ToLower() ?
        //                                Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 :
        //                            DataType.ToLower() == "Double".ToLower() ?
        //                                Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 : false) :
        //                        Operation == "<=" ?
        //                            (DataType.ToLower() == "DateTime".ToLower() ?
        //                                (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 ||
        //                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) :
        //                            DataType.ToLower() == "Double".ToLower() ?
        //                                (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 ||
        //                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) : false) :
        //                        Operation == "==" ?
        //                            x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower() :
        //                        Operation == "!=" ?
        //                            x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != Value.ToString().ToLower() : false) : false).ToList();

        //            foreach (object Record in TableRecords)
        //            {
        //                object PrimaryKeyValue = Record.GetType().GetProperty(Path[2]).GetValue(Record, null);

        //                LoopForPath(Path, 3, _dbContext, PrimaryKeyValue, OutPutIds);
        //            }
        //        }
        //        else if (Rule.dynamicAttId != null && Rule.IsDynamic)
        //        {
        //            TLIdynamicAtt DynamicAttribute = _unitOfWork.DynamicAttRepository
        //                .GetByID(Rule.dynamicAttId.Value);
        //            if (!DynamicAttribute.LibraryAtt)
        //            {
        //                List<TLIdynamicAttInstValue> DynamicAttValues = new List<TLIdynamicAttInstValue>();
        //                if (Rule.OperationValueBoolean != null)
        //                {
        //                    DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
        //                        .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
        //                            (x.ValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false)).ToList();
        //                }
        //                else if (Rule.OperationValueDateTime != null)
        //                {
        //                    DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
        //                        .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
        //                            (x.ValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false)).ToList();
        //                }
        //                else if (Rule.OperationValueDouble != null)
        //                {
        //                    DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
        //                        .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
        //                            (x.ValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false)).ToList();
        //                }
        //                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
        //                {
        //                    DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
        //                        .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
        //                            (!string.IsNullOrEmpty(x.ValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToString().ToLower() : false)).ToList();
        //                }
        //                if (DynamicAttValues != null)
        //                {
        //                    TableRecords = Mapper.Map<List<object>>(_dbContext.GetType()
        //                        .GetProperties().FirstOrDefault(x => x.Name.ToLower() == Path[1].ToLower()).GetValue(_dbContext, null))
        //                            .Where(x => DynamicAttValues.Exists(y =>
        //                                y.InventoryId.ToString() == x.GetType().GetProperty("Id").GetValue(x, null).ToString() ? (
        //                                    (y.ValueBoolean != null ? (
        //                                        Operation == "==" ? y.ValueBoolean.ToString().ToLower() == Value.ToString().ToLower() :
        //                                        Operation == "!=" ? y.ValueBoolean.ToString().ToLower() != Value.ToString().ToLower() : false) : false) ||
        //                                    (y.ValueDateTime != null ? (
        //                                        Operation == "==" ? y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower() :
        //                                        Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 :
        //                                        Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 ||
        //                                            y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
        //                                        Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 :
        //                                        Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 ||
        //                                            y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
        //                                        Operation == "!=" ? y.ValueDateTime.ToString().ToLower() != Value.ToString().ToLower() : false) : false) ||
        //                                    (y.ValueDouble != null ? (
        //                                        Operation == "==" ? y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower() :
        //                                        Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 :
        //                                        Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 ||
        //                                            y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
        //                                        Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 :
        //                                        Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 ||
        //                                            y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
        //                                        Operation == "!=" ? y.ValueDouble.ToString().ToLower() != Value.ToString().ToLower() : false) : false) ||
        //                                    (!string.IsNullOrEmpty(y.ValueString) ? (
        //                                        Operation == "==" ? y.ValueString.ToLower() == Value.ToString().ToLower() :
        //                                        Operation == "!=" ? y.ValueString.ToLower() != Value.ToString().ToLower() : false) : false)
        //                                ) : false)).ToList();

        //                    foreach (object Record in TableRecords)
        //                    {
        //                        object PrimaryKeyValue = Record.GetType().GetProperty(Path[2]).GetValue(Record, null);

        //                        LoopForPath(Path, 3, _dbContext, PrimaryKeyValue, OutPutIds);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                List<TLIdynamicAttLibValue> DynamicAttValues = new List<TLIdynamicAttLibValue>();
        //                if (Rule.OperationValueBoolean != null)
        //                {
        //                    DynamicAttValues = _unitOfWork.DynamicAttLibRepository
        //                        .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
        //                            (x.ValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false)).ToList();
        //                }
        //                else if (Rule.OperationValueDateTime != null)
        //                {
        //                    DynamicAttValues = _unitOfWork.DynamicAttLibRepository
        //                        .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
        //                            (x.ValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false)).ToList();
        //                }
        //                else if (Rule.OperationValueDouble != null)
        //                {
        //                    DynamicAttValues = _unitOfWork.DynamicAttLibRepository
        //                        .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
        //                            (x.ValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false)).ToList();
        //                }
        //                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
        //                {
        //                    DynamicAttValues = _unitOfWork.DynamicAttLibRepository
        //                        .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
        //                            (!string.IsNullOrEmpty(x.ValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)).ToList();
        //                }
        //                if (DynamicAttValues != null)
        //                {
        //                    TableRecords = Mapper.Map<List<object>>(_dbContext.GetType()
        //                        .GetProperties().FirstOrDefault(x => x.Name.ToLower() == Path[1].ToLower()).GetValue(_dbContext, null))
        //                            .Where(x => DynamicAttValues.FirstOrDefault(y =>
        //                                y.InventoryId.ToString() == x.GetType().GetProperty("Id").GetValue(x, null).ToString() ? (
        //                                    (y.ValueBoolean != null ? (
        //                                        Operation == "==" ? y.ValueBoolean.ToString().ToLower() == Value.ToString().ToLower() :
        //                                        Operation == "!=" ? y.ValueBoolean.ToString().ToLower() != Value.ToString().ToLower() : false) : false) ||
        //                                    (y.ValueDateTime != null ? (
        //                                        Operation == "==" ? y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower() :
        //                                        Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 :
        //                                        Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 ||
        //                                            y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
        //                                        Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 :
        //                                        Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 ||
        //                                            y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
        //                                        Operation == "!=" ? y.ValueDateTime.ToString().ToLower() != Value.ToString().ToLower() : false
        //                                    ) : false) ||
        //                                    (y.ValueDouble != null ? (
        //                                        Operation == "==" ? y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower() :
        //                                        Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 :
        //                                        Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 ||
        //                                            y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
        //                                        Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 :
        //                                        Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 ||
        //                                            y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
        //                                        Operation == "!=" ? y.ValueDouble.ToString().ToLower() != Value.ToString().ToLower() : false
        //                                    ) : false) ||
        //                                    (!string.IsNullOrEmpty(y.ValueString) ?
        //                                        (Operation == "==" ? y.ValueString.ToLower() == Value.ToString().ToLower() :
        //                                        Operation == "!=" ? y.ValueString.ToLower() != Value.ToString().ToLower() : false) : false)) : false) != null).ToList();

        //                    foreach (object Record in TableRecords)
        //                    {
        //                        object PrimaryKeyValue = Record.GetType().GetProperty(Path[2]).GetValue(Record, null);

        //                        LoopForPath(Path, 3, _dbContext, PrimaryKeyValue, OutPutIds);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    OutPutIds = OutPutIds.Distinct().ToList();
        //    return OutPutIds;
        //}
        //public void AddDefaultValues(AddDependencyViewModel AddDependencyViewModel, int DynamicAttId)
        //{
        //    try
        //    {
        //        if (AddDependencyViewModel != null)
        //        {
        //            if (AddDependencyViewModel.BooleanDefaultValue != null || AddDependencyViewModel.DoubleDefaultValue != null ||
        //                AddDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(AddDependencyViewModel.StringDefaultValue))
        //            {
        //                List<int> RecordsIds = new List<int>();

        //                if (AddDependencyViewModel.LibraryAtt)
        //                {
        //                    RecordsIds = GetLibraryRecordsIds(AddDependencyViewModel.TableName);
        //                    List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

        //                    foreach (int RecordId in RecordsIds)
        //                    {
        //                        ListToAdd.Add(new TLIdynamicAttLibValue
        //                        {
        //                            disable = false,
        //                            DynamicAttId = DynamicAttId,
        //                            InventoryId = RecordId,
        //                            tablesNamesId = AddDependencyViewModel.tablesNamesId,
        //                            ValueBoolean = AddDependencyViewModel.BooleanDefaultValue,
        //                            ValueDateTime = AddDependencyViewModel.DateTimeDefaultValue,
        //                            ValueDouble = AddDependencyViewModel.DoubleDefaultValue,
        //                            ValueString = AddDependencyViewModel.StringDefaultValue
        //                        });
        //                    }

        //                    _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
        //                    _unitOfWork.SaveChanges();
        //                }
        //                else
        //                {
        //                    RecordsIds = GetInstallationRecordsIds(AddDependencyViewModel.TableName);
        //                    List<TLIdynamicAttInstValue> ListToAdd = new List<TLIdynamicAttInstValue>();

        //                    foreach (int RecordId in RecordsIds)
        //                    {
        //                        ListToAdd.Add(new TLIdynamicAttInstValue
        //                        {
        //                            disable = false,
        //                            DynamicAttId = DynamicAttId,
        //                            InventoryId = RecordId,
        //                            tablesNamesId = AddDependencyViewModel.tablesNamesId,
        //                            ValueBoolean = AddDependencyViewModel.BooleanDefaultValue,
        //                            ValueDateTime = AddDependencyViewModel.DateTimeDefaultValue,
        //                            ValueDouble = AddDependencyViewModel.DoubleDefaultValue,
        //                            ValueString = AddDependencyViewModel.StringDefaultValue
        //                        });
        //                    }

        //                    _unitOfWork.DynamicAttInstValueRepository.AddRange(ListToAdd);
        //                    _unitOfWork.SaveChanges();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (addDependencyInstViewModel.BooleanDefaultValue != null || addDependencyInstViewModel.DoubleDefaultValue != null ||
        //                addDependencyInstViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyInstViewModel.StringDefaultValue))
        //            {
        //                List<int> RecordsIds = new List<int>();

        //                if (addDependencyInstViewModel.LibraryAtt)
        //                {
        //                    RecordsIds = GetLibraryRecordsIds(addDependencyInstViewModel.TableName);
        //                    List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

        //                    foreach (int RecordId in RecordsIds)
        //                    {
        //                        ListToAdd.Add(new TLIdynamicAttLibValue
        //                        {
        //                            disable = false,
        //                            DynamicAttId = DynamicAttId,
        //                            InventoryId = RecordId,
        //                            tablesNamesId = addDependencyInstViewModel.tablesNamesId,
        //                            ValueBoolean = addDependencyInstViewModel.BooleanDefaultValue,
        //                            ValueDateTime = addDependencyInstViewModel.DateTimeDefaultValue,
        //                            ValueDouble = addDependencyInstViewModel.DoubleDefaultValue,
        //                            ValueString = addDependencyInstViewModel.StringDefaultValue
        //                        });
        //                    }
        //                    _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
        //                    _unitOfWork.SaveChanges();
        //                }
        //                else
        //                {
        //                    RecordsIds = GetInstallationRecordsIds(addDependencyInstViewModel.TableName);
        //                    List<TLIdynamicAttInstValue> ListToAdd = new List<TLIdynamicAttInstValue>();

        //                    foreach (int RecordId in RecordsIds)
        //                    {
        //                        ListToAdd.Add(new TLIdynamicAttInstValue
        //                        {
        //                            disable = false,
        //                            DynamicAttId = DynamicAttId,
        //                            InventoryId = RecordId,
        //                            tablesNamesId = addDependencyInstViewModel.tablesNamesId,
        //                            ValueBoolean = addDependencyInstViewModel.BooleanDefaultValue,
        //                            ValueDateTime = addDependencyInstViewModel.DateTimeDefaultValue,
        //                            ValueDouble = addDependencyInstViewModel.DoubleDefaultValue,
        //                            ValueString = addDependencyInstViewModel.StringDefaultValue
        //                        });
        //                    }
        //                    _unitOfWork.DynamicAttInstValueRepository.AddRange(ListToAdd);
        //                    _unitOfWork.SaveChanges();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        throw;
        //    }
        //}
        public ApiResponse GetAllDataTypes()
        {
            List<Data_Type> Data_Types = _DbContext.Data_Types.ToList();
            return new ApiResponse(Data_Types, "Succeed");
        }
        public ApiResponse GetAllOperations()
        {
            List<Operation> Operations = _DbContext.Operations.ToList();
            return new ApiResponse(Operations, "Succeed");
        }
    }
}
