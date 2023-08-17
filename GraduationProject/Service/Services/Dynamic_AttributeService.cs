using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.DynamicAttribute;
using GraduationProject.DataBase.ViewModels.DynamicAttribute.Dependency;
using GraduationProject.DataBase.ViewModels.Patient;
using GraduationProject.DataBase.ViewModels.User;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Transactions;
using static GraduationProject.DataBase.Helpers.Constants;
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
        public ApiResponse GetAllDynamicAttributes(ComplexFilter Filter, int ClinicId, bool? OnlyHealthStandards)
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
        public ApiResponse AddNewDynamicAttribute(AddDynamic_AttributeViewModel NewDynamicAttribute)
        {
            using (TransactionScope Transaction = new TransactionScope())
            {
                List<PatientWithMedicalInfo> PatientsFullData = new List<PatientWithMedicalInfo>();

                // Get All The Patients in This Clinic For Adding The Default Value OR The Dependency Value To Them..
                List<PatientViewModel> AllClinicPatients = _Mapper.Map<List<PatientViewModel>>(_DbContext.Appointments
                    .Include(x => x.Doctor).Include(x => x.Patient)
                    .Where(x => x.Doctor.ClinicId == NewDynamicAttribute.ClinicId)
                    .Select(x => x.Patient).ToList());

                List<Medical_Information> MedicalInformationFullData = _DbContext.Medical_Informations
                    .Where(x => AllClinicPatients.Exists(y => y.UserInformation.Id == x.PatientId)).ToList();

                List<Medicine> MedicinesFullData = _DbContext.Medicines.Where(x => MedicalInformationFullData
                    .Exists(y => y.Id == x.MedicalInfoId)).ToList();

                foreach (PatientViewModel Patient in AllClinicPatients)
                {
                    Medical_Information? MedicalInfo = MedicalInformationFullData
                        .FirstOrDefault(x => x.PatientId == Patient.UserInformation.Id);

                    if (MedicalInfo != null)
                    {
                        Medicine? MedicineInfo = MedicinesFullData
                            .FirstOrDefault(x => x.MedicalInfoId == MedicalInfo.Id);

                        if (MedicineInfo != null)
                        {
                            PatientWithMedicalInfo OnePatientFullData = new PatientWithMedicalInfo()
                            {
                                Id = Patient.Id,
                                Birthdate = Patient.Birthdate,
                                Gender = Patient.Gender,
                                Address = Patient.Address,
                                VisitCount = Patient.VisitCount,
                                UserId = Patient.UserInformation.Id,
                                Name = Patient.UserInformation.Name,
                                First_Name = Patient.UserInformation.First_Name,
                                Last_Name = Patient.UserInformation.Last_Name,
                                Phone_Number = Patient.UserInformation.Phone_Number,
                                Telephone_Number = Patient.UserInformation.Telephone_Number,
                                Email = Patient.UserInformation.Email,
                                Medical_InfoId = MedicalInfo.Id,
                                Height = MedicalInfo.Height,
                                BGroup = MedicalInfo.BGroup,
                                Pulse = MedicalInfo.Pulse,
                                Weight = MedicalInfo.Weight,
                                BPressure = MedicalInfo.BPressure,
                                Respiration = MedicalInfo.Respiration,
                                Diet = MedicalInfo.Diet,
                                MedicineName = MedicineInfo.Name,
                                MedicineDescription = MedicineInfo.Description
                            };

                            PatientsFullData.Add(OnePatientFullData);
                        }
                        else
                        {
                            PatientWithMedicalInfo OnePatientFullData = new PatientWithMedicalInfo()
                            {
                                Id = Patient.Id,
                                Birthdate = Patient.Birthdate,
                                Gender = Patient.Gender,
                                Address = Patient.Address,
                                VisitCount = Patient.VisitCount,
                                UserId = Patient.UserInformation.Id,
                                Name = Patient.UserInformation.Name,
                                First_Name = Patient.UserInformation.First_Name,
                                Last_Name = Patient.UserInformation.Last_Name,
                                Phone_Number = Patient.UserInformation.Phone_Number,
                                Telephone_Number = Patient.UserInformation.Telephone_Number,
                                Email = Patient.UserInformation.Email,
                                Medical_InfoId = MedicalInfo.Id,
                                Height = MedicalInfo.Height,
                                BGroup = MedicalInfo.BGroup,
                                Pulse = MedicalInfo.Pulse,
                                Weight = MedicalInfo.Weight,
                                BPressure = MedicalInfo.BPressure,
                                Respiration = MedicalInfo.Respiration,
                                Diet = MedicalInfo.Diet
                            };

                            PatientsFullData.Add(OnePatientFullData);
                        }
                    }
                }

                // General Information..
                Dynamic_Attribute NewNewDynamicAttributeEntity = _Mapper.Map<Dynamic_Attribute>(NewDynamicAttribute);

                NewNewDynamicAttributeEntity.Disable = false;

                _DbContext.Dynamic_Attributes.Add(NewNewDynamicAttributeEntity);
                _DbContext.SaveChanges();

                // General Validation..
                if (NewDynamicAttribute.GeneralValidation != null)
                {
                    Validation NewValidationEntity = _Mapper.Map<Validation>(NewDynamicAttribute.GeneralValidation);

                    NewValidationEntity.DynamicAttributeId = NewNewDynamicAttributeEntity.Id;

                    _DbContext.Validations.Add(NewValidationEntity);
                    _DbContext.SaveChanges();
                }

                // Dependency Information..
                if (NewDynamicAttribute.Dependency != null)
                {
                    // Get All Static And Dynamic Attributes..
                    List<Static_Attribute> AllStaticAttributes = _DbContext.Static_Attributes.ToList();

                    List<Dynamic_Attribute> AllDynamicAttributes = _DbContext.Dynamic_Attributes
                        .Where(x => !x.Disable && x.ClinicId == NewDynamicAttribute.ClinicId).ToList();

                    // First We Have The Dependency Main Groups Conditions
                    foreach (List<AddDependencyGroupsRules> DependencySingleGroupRules in NewDynamicAttribute.Dependency.DependencyGroupsRules)
                    {
                        // Copy Of AllClinicPatients List..
                        List<PatientViewModel> CopyOfAllClinicPatients = AllClinicPatients;

                        Row NewRowEntity = new Row();

                        _DbContext.Rows.Add(NewRowEntity);
                        _DbContext.SaveChanges();

                        foreach (AddDependencyGroupsRules DependencySingleGroupSingleRule in DependencySingleGroupRules)
                        {
                            Rule NewRuleEntity = _Mapper.Map<Rule>(DependencySingleGroupSingleRule);

                            NewRuleEntity.NewDynamicAttributeId = NewNewDynamicAttributeEntity.Id;
                            NewRuleEntity.ClinicId = NewDynamicAttribute.ClinicId;

                            _DbContext.Rules.Add(NewRuleEntity);
                            _DbContext.SaveChanges();

                            Row_Rule NewRow_RuleEntity = new Row_Rule()
                            {
                                RowId = NewRowEntity.Id,
                                RuleId = NewRuleEntity.Id,
                                IsDeleted = false
                            };

                            _DbContext.Row_Rules.Add(NewRow_RuleEntity);
                            _DbContext.SaveChanges();

                            if (!string.IsNullOrEmpty(NewDynamicAttribute.Dependency.DependencyValue))
                            {
                                if (DependencySingleGroupSingleRule.OperationValueBoolean != null)
                                {
                                    PatientsFullData = PatientsFullData.Where(x => bool.Parse(x.GetType()
                                        .GetProperty(DependencySingleGroupSingleRule.StaticAttributeId != null ?
                                            AllStaticAttributes.FirstOrDefault(StaticAttribute => StaticAttribute.Id == DependencySingleGroupSingleRule.StaticAttributeId).Key :
                                            AllDynamicAttributes.FirstOrDefault(DynamicAttribute => DynamicAttribute.Id == DependencySingleGroupSingleRule.DynamicAttributeId).Key)
                                        .GetValue(x, null).ToString()) == DependencySingleGroupSingleRule.OperationValueBoolean).ToList();
                                }
                                else if (DependencySingleGroupSingleRule.OperationValueDateTime != null)
                                {
                                    PatientsFullData = PatientsFullData.Where(x => DateTime.Parse(x.GetType()
                                        .GetProperty(DependencySingleGroupSingleRule.StaticAttributeId != null ?
                                            AllStaticAttributes.FirstOrDefault(StaticAttribute => StaticAttribute.Id == DependencySingleGroupSingleRule.StaticAttributeId).Key :
                                            AllDynamicAttributes.FirstOrDefault(DynamicAttribute => DynamicAttribute.Id == DependencySingleGroupSingleRule.DynamicAttributeId).Key)
                                        .GetValue(x, null).ToString()) == DependencySingleGroupSingleRule.OperationValueDateTime).ToList();
                                }
                                else if (DependencySingleGroupSingleRule.OperationValueDouble != null)
                                {
                                    PatientsFullData = PatientsFullData.Where(x => double.Parse(x.GetType()
                                        .GetProperty(DependencySingleGroupSingleRule.StaticAttributeId != null ?
                                            AllStaticAttributes.FirstOrDefault(StaticAttribute => StaticAttribute.Id == DependencySingleGroupSingleRule.StaticAttributeId).Key :
                                            AllDynamicAttributes.FirstOrDefault(DynamicAttribute => DynamicAttribute.Id == DependencySingleGroupSingleRule.DynamicAttributeId).Key)
                                        .GetValue(x, null).ToString()) == DependencySingleGroupSingleRule.OperationValueDouble).ToList();
                                }
                                else if (DependencySingleGroupSingleRule.OperationValueString != null)
                                {
                                    PatientsFullData = PatientsFullData.Where(x => x.GetType()
                                        .GetProperty(DependencySingleGroupSingleRule.StaticAttributeId != null ?
                                            AllStaticAttributes.FirstOrDefault(StaticAttribute => StaticAttribute.Id == DependencySingleGroupSingleRule.StaticAttributeId).Key :
                                            AllDynamicAttributes.FirstOrDefault(DynamicAttribute => DynamicAttribute.Id == DependencySingleGroupSingleRule.DynamicAttributeId).Key)
                                        .GetValue(x, null).ToString() == DependencySingleGroupSingleRule.OperationValueString).ToList();
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(NewDynamicAttribute.Dependency.DependencyValue) && CopyOfAllClinicPatients.Count() > 0)
                        {
                            Data_Type? DynamicAttributeDataType = _DbContext.Data_Types
                                .FirstOrDefault(x => x.Id == NewDynamicAttribute.DataTypeId);

                            List<Dynamic_Attribute_Value> PatientDynamicAttributeValues = new List<Dynamic_Attribute_Value>();

                            if (DynamicAttributeDataType != null)
                            {
                                if (DynamicAttributeDataType.Name.ToLower() == DataTypes.Date.ToString().ToLower())
                                {
                                    foreach (PatientViewModel Patient in CopyOfAllClinicPatients)
                                    {
                                        Dynamic_Attribute_Value PatientDynamicAttributeValue = new Dynamic_Attribute_Value()
                                        {
                                            PatientId = Patient.Id,
                                            DynamicAttributeId = NewNewDynamicAttributeEntity.Id,
                                            ValueDateTime = DateTime.Parse(NewDynamicAttribute.Dependency.DependencyValue),
                                            Disable = false,
                                            IsDeleted = false
                                        };

                                        PatientDynamicAttributeValues.Add(PatientDynamicAttributeValue);
                                    }
                                }
                                else if (DynamicAttributeDataType.Name.ToLower() == DataTypes.Boolean.ToString().ToLower())
                                {
                                    foreach (PatientViewModel Patient in CopyOfAllClinicPatients)
                                    {
                                        Dynamic_Attribute_Value PatientDynamicAttributeValue = new Dynamic_Attribute_Value()
                                        {
                                            PatientId = Patient.Id,
                                            DynamicAttributeId = NewNewDynamicAttributeEntity.Id,
                                            ValueBoolean = bool.Parse(NewDynamicAttribute.Dependency.DependencyValue),
                                            Disable = false,
                                            IsDeleted = false
                                        };

                                        PatientDynamicAttributeValues.Add(PatientDynamicAttributeValue);
                                    }
                                }
                                else if (DynamicAttributeDataType.Name.ToLower() == DataTypes.Text.ToString().ToLower())
                                {
                                    foreach (PatientViewModel Patient in CopyOfAllClinicPatients)
                                    {
                                        Dynamic_Attribute_Value PatientDynamicAttributeValue = new Dynamic_Attribute_Value()
                                        {
                                            PatientId = Patient.Id,
                                            DynamicAttributeId = NewNewDynamicAttributeEntity.Id,
                                            ValueString = NewDynamicAttribute.Dependency.DependencyValue,
                                            Disable = false,
                                            IsDeleted = false
                                        };

                                        PatientDynamicAttributeValues.Add(PatientDynamicAttributeValue);
                                    }
                                }
                                else if (DynamicAttributeDataType.Name.ToLower() == DataTypes.Number.ToString().ToLower())
                                {
                                    foreach (PatientViewModel Patient in CopyOfAllClinicPatients)
                                    {
                                        Dynamic_Attribute_Value PatientDynamicAttributeValue = new Dynamic_Attribute_Value()
                                        {
                                            PatientId = Patient.Id,
                                            DynamicAttributeId = NewNewDynamicAttributeEntity.Id,
                                            ValueDouble = double.Parse(NewDynamicAttribute.Dependency.DependencyValue),
                                            Disable = false,
                                            IsDeleted = false
                                        };

                                        PatientDynamicAttributeValues.Add(PatientDynamicAttributeValue);
                                    }
                                }
                            }

                            _DbContext.Dynamic_Attribute_Values.AddRange(PatientDynamicAttributeValues);
                            _DbContext.SaveChanges();
                        }
                    }

                    if (NewDynamicAttribute.Dependency.DependencyValidation != null)
                    {
                        Dependency NewDependecnyEntity = _Mapper.Map<Dependency>(NewDynamicAttribute.Dependency.DependencyValidation);

                        NewDependecnyEntity.DynamicAttributeId = NewNewDynamicAttributeEntity.Id;

                        _DbContext.Dependencies.Add(NewDependecnyEntity);
                        _DbContext.SaveChanges();
                    }
                }

                if (!string.IsNullOrEmpty(NewDynamicAttribute.DefaultValue))
                {
                    List<PatientViewModel> ClinicPatientsThatDoesHaveDependencyValue = _Mapper.Map<List<PatientViewModel>>(_DbContext.Dynamic_Attribute_Values
                        .Include(x => x.Patient)
                        .Where(x => x.DynamicAttributeId == NewNewDynamicAttributeEntity.Id)
                        .Select(x => x.Patient).ToList());

                    List<PatientViewModel> RemainPatients = AllClinicPatients.Except(ClinicPatientsThatDoesHaveDependencyValue).ToList();

                    Data_Type? DynamicAttributeDataType = _DbContext.Data_Types
                            .FirstOrDefault(x => x.Id == NewDynamicAttribute.DataTypeId);

                    List<Dynamic_Attribute_Value> PatientDynamicAttributeValues = new List<Dynamic_Attribute_Value>();

                    if (DynamicAttributeDataType != null)
                    {
                        if (DynamicAttributeDataType.Name.ToLower() == DataTypes.Date.ToString().ToLower())
                        {
                            foreach (PatientViewModel Patient in RemainPatients)
                            {
                                Dynamic_Attribute_Value PatientDynamicAttributeValue = new Dynamic_Attribute_Value()
                                {
                                    PatientId = Patient.Id,
                                    DynamicAttributeId = NewNewDynamicAttributeEntity.Id,
                                    ValueDateTime = DateTime.Parse(NewDynamicAttribute.DefaultValue),
                                    Disable = false,
                                    IsDeleted = false
                                };

                                PatientDynamicAttributeValues.Add(PatientDynamicAttributeValue);
                            }
                        }
                        else if (DynamicAttributeDataType.Name.ToLower() == DataTypes.Boolean.ToString().ToLower())
                        {
                            foreach (PatientViewModel Patient in RemainPatients)
                            {
                                Dynamic_Attribute_Value PatientDynamicAttributeValue = new Dynamic_Attribute_Value()
                                {
                                    PatientId = Patient.Id,
                                    DynamicAttributeId = NewNewDynamicAttributeEntity.Id,
                                    ValueBoolean = bool.Parse(NewDynamicAttribute.DefaultValue),
                                    Disable = false,
                                    IsDeleted = false
                                };

                                PatientDynamicAttributeValues.Add(PatientDynamicAttributeValue);
                            }
                        }
                        else if (DynamicAttributeDataType.Name.ToLower() == DataTypes.Text.ToString().ToLower())
                        {
                            foreach (PatientViewModel Patient in RemainPatients)
                            {
                                Dynamic_Attribute_Value PatientDynamicAttributeValue = new Dynamic_Attribute_Value()
                                {
                                    PatientId = Patient.Id,
                                    DynamicAttributeId = NewNewDynamicAttributeEntity.Id,
                                    ValueString = NewDynamicAttribute.DefaultValue,
                                    Disable = false,
                                    IsDeleted = false
                                };

                                PatientDynamicAttributeValues.Add(PatientDynamicAttributeValue);
                            }
                        }
                        else if (DynamicAttributeDataType.Name.ToLower() == DataTypes.Number.ToString().ToLower())
                        {
                            foreach (PatientViewModel Patient in RemainPatients)
                            {
                                Dynamic_Attribute_Value PatientDynamicAttributeValue = new Dynamic_Attribute_Value()
                                {
                                    PatientId = Patient.Id,
                                    DynamicAttributeId = NewNewDynamicAttributeEntity.Id,
                                    ValueDouble = double.Parse(NewDynamicAttribute.DefaultValue),
                                    Disable = false,
                                    IsDeleted = false
                                };

                                PatientDynamicAttributeValues.Add(PatientDynamicAttributeValue);
                            }
                        }
                    }

                    _DbContext.Dynamic_Attribute_Values.AddRange(PatientDynamicAttributeValues);
                    _DbContext.SaveChanges();
                }

                Transaction.Complete();
                return new ApiResponse(true, "Succeed");
            }
        }
        public ApiResponse EditDynamicAttribute(EditDynamic_AttributeViewModel EditDynamicAttribute)
        {
            Dynamic_Attribute? DynamicAttributeEntity = _DbContext.Dynamic_Attributes
                .FirstOrDefault(x => x.Id == EditDynamicAttribute.Id);

            if (DynamicAttributeEntity == null)
                return new ApiResponse(false, $"No Dynamic Attribute Found With This Id: ({EditDynamicAttribute.Id})");

            bool CheckDynamicAttributeKeyIfDuplicate = _DbContext.Dynamic_Attributes
                .Any(x => x.Key.ToLower() == EditDynamicAttribute.Key.ToLower() && x.ClinicId == DynamicAttributeEntity.ClinicId &&
                    x.Id != EditDynamicAttribute.Id);

            if (CheckDynamicAttributeKeyIfDuplicate)
                return new ApiResponse(false, $"This Dynamic Attribute's Key: ({EditDynamicAttribute.Key}) is Already Exist in This Clinic");

            DynamicAttributeEntity.Key = EditDynamicAttribute.Key;
            DynamicAttributeEntity.Description = EditDynamicAttribute.Description;

            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse ChangeDynamicAttributeRequiredStatus(int DynmaicAttributeId)
        {
            Dynamic_Attribute? DynamicAttributeEntity = _DbContext.Dynamic_Attributes
                .FirstOrDefault(x => x.Id == DynmaicAttributeId);

            if (DynamicAttributeEntity == null)
                return new ApiResponse(false, $"No Dynamic Attribute Found With This Id: ({DynmaicAttributeId})");

            DynamicAttributeEntity.Required = !DynamicAttributeEntity.Required;

            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse ChangeDynamicAttributeDisableStatus(int DynmaicAttributeId)
        {
            Dynamic_Attribute? DynamicAttributeEntity = _DbContext.Dynamic_Attributes
                .FirstOrDefault(x => x.Id == DynmaicAttributeId);

            if (DynamicAttributeEntity == null)
                return new ApiResponse(false, $"No Dynamic Attribute Found With This Id: ({DynmaicAttributeId})");

            DynamicAttributeEntity.Disable = !DynamicAttributeEntity.Disable;

            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse ChangeDynamicAttributeHealthStandardStatus(int DynmaicAttributeId)
        {
            Dynamic_Attribute? DynamicAttributeEntity = _DbContext.Dynamic_Attributes
                .FirstOrDefault(x => x.Id == DynmaicAttributeId);

            if (DynamicAttributeEntity == null)
                return new ApiResponse(false, $"No Dynamic Attribute Found With This Id: ({DynmaicAttributeId})");

            DynamicAttributeEntity.IsHealthStandard = !DynamicAttributeEntity.IsHealthStandard;

            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse GetAllDependencyColumns(int ClinicId, string? SearchQuery)
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                List<DependencyColumnsViewModel> OutPut = new List<DependencyColumnsViewModel>();

                List<DependencyColumnsViewModel> DynamicAttributes = _Mapper.Map<List<DependencyColumnsViewModel>>(_DbContext.Dynamic_Attributes
                    .Include(x => x.DataType)
                    .Where(x => x.ClinicId == ClinicId && !x.IsDeleted && !x.Disable).ToList());

                List<DependencyColumnsViewModel> StaticAttribute = _Mapper.Map<List<DependencyColumnsViewModel>>(_DbContext.Static_Attributes
                    .Include(x => x.DataType)
                    .Where(x => x.ClinicId == ClinicId && !x.IsDeleted && x.Enable).ToList());

                OutPut = DynamicAttributes.Concat(StaticAttribute).ToList();

                return new ApiResponse(OutPut, "Succeed");
            }
            else
            {
                List<DependencyColumnsViewModel> OutPut = new List<DependencyColumnsViewModel>();

                List<DependencyColumnsViewModel> DynamicAttributes = _Mapper.Map<List<DependencyColumnsViewModel>>(_DbContext.Dynamic_Attributes
                    .Include(x => x.DataType)
                    .Where(x => x.ClinicId == ClinicId && !x.IsDeleted && !x.Disable && x.Key.ToLower().StartsWith(SearchQuery.ToLower())).ToList());

                List<DependencyColumnsViewModel> StaticAttribute = _Mapper.Map<List<DependencyColumnsViewModel>>(_DbContext.Static_Attributes
                    .Include(x => x.DataType)
                    .Where(x => x.ClinicId == ClinicId && !x.IsDeleted && x.Enable && x.Label.ToLower().StartsWith(SearchQuery.ToLower())).ToList());

                OutPut = DynamicAttributes.Concat(StaticAttribute).ToList();

                return new ApiResponse(OutPut, "Succeed");
            }
        }
    }
}
