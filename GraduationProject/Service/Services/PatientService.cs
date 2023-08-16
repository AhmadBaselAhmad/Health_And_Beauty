using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.DynamicAttribute.Dependency;
using GraduationProject.DataBase.ViewModels.DynamicAttributeValue;
using GraduationProject.DataBase.ViewModels.Medical_Information;
using GraduationProject.DataBase.ViewModels.Patient;
using GraduationProject.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Transactions;

namespace GraduationProject.Service.Services
{
    public class PatientService : IPatientService
    {
        private GraduationProjectDbContext _DbContext;
        private readonly IMapper _Mapper;
        public PatientService(GraduationProjectDbContext DbContext, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Mapper = Mapper;
        }
        public ApiResponse AddPatientMedicalInfo(AddMedical_InformationsViewModel PatientMedicalInformationViewModel)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                Patient? Patient = _DbContext.Patients
                    .FirstOrDefault(x => x.UserId == PatientMedicalInformationViewModel.PatientId);

                Medical_Information? CheckNameDuplicate = _DbContext.Medical_Informations
                    .FirstOrDefault(x => x.PatientId == PatientMedicalInformationViewModel.PatientId);

                if (CheckNameDuplicate != null)
                    return new ApiResponse(false, $"You Can't Add Another Medical Information For This Patient Id: " +
                        $"({PatientMedicalInformationViewModel.PatientId})");

                Medical_Information PatientMedical_InformationEntity = _Mapper.Map<Medical_Information>(PatientMedicalInformationViewModel);

                _DbContext.Medical_Informations.Add(PatientMedical_InformationEntity);
                _DbContext.SaveChanges();

                List<Dynamic_Attribute> DynamicAttributes = _DbContext.Dynamic_Attributes
                    .Where(x => !x.Disable).ToList();

                List<Static_Attribute> StaticAttributes = _DbContext.Static_Attributes.Where(x => x.Enable).ToList();

                foreach (Dynamic_Attribute DynamicAttribute in DynamicAttributes)
                {
                    //
                    // General Validation..
                    //

                    Validation? CheckGeneralValidation = _DbContext.Validations
                        .Include(x => x.Operation)
                        .FirstOrDefault(x => x.DynamicAttributeId == DynamicAttribute.Id);

                    if (CheckGeneralValidation != null)
                    {
                        AddDynamicAttributeValue? CheckIfDynamicAttributeHasAValue = PatientMedicalInformationViewModel.DynamicAttributesValues
                            .FirstOrDefault(x => x.Id == DynamicAttribute.Id);

                        if (CheckIfDynamicAttributeHasAValue != null)
                        {
                            if (!string.IsNullOrEmpty(CheckGeneralValidation.ValueString))
                            {
                                if (CheckGeneralValidation.Operation.Name == "==")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.StringValue.ToLower() == CheckGeneralValidation.ValueString.ToLower()))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Equal to {CheckGeneralValidation.ValueString}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "!=")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.StringValue.ToLower() != CheckGeneralValidation.ValueString.ToLower()))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must not be Equal to {CheckGeneralValidation.ValueString}");
                                }
                            }
                            else if (CheckGeneralValidation.ValueDouble != null)
                            {
                                if (CheckGeneralValidation.Operation.Name == "==")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.NumberValue == CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Equal to {CheckGeneralValidation.ValueDouble}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "!=")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.NumberValue != CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must not be Equal to {CheckGeneralValidation.ValueDouble}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == ">")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.NumberValue > CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Bigger Than {CheckGeneralValidation.ValueDouble}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == ">=")
                                {
                                    if (!(CheckGeneralValidation.ValueDouble > CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Bigger Than or Equal to {CheckGeneralValidation.ValueDouble}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "<")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.NumberValue < CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Smaller Than {CheckGeneralValidation.ValueDouble}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "<=")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.NumberValue <= CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Smaller Than or Equal to {CheckGeneralValidation.ValueDouble}");
                                }
                            }
                            else if (CheckGeneralValidation.ValueDateTime != null)
                            {
                                DateOnly DateTimeValue = DateOnly.FromDateTime(CheckIfDynamicAttributeHasAValue.DateTimeValue.Value);
                                DateOnly ValueDateTime = DateOnly.FromDateTime(CheckGeneralValidation.ValueDateTime.Value);

                                if (CheckGeneralValidation.Operation.Name == "==")
                                {
                                    if (!(DateTimeValue == ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Equal to {ValueDateTime}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "!=")
                                {
                                    if (!(DateTimeValue != ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must not be Equal to {ValueDateTime}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == ">")
                                {
                                    if (!(DateTimeValue > ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Bigger Than {ValueDateTime}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == ">=")
                                {
                                    if (!(ValueDateTime > ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Bigger Than or Equal to {ValueDateTime}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "<")
                                {
                                    if (!(DateTimeValue < ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Smaller Than {ValueDateTime}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "<=")
                                {
                                    if (!(DateTimeValue <= ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Smaller Than or Equal to {ValueDateTime}");
                                }
                            }
                            else if (CheckGeneralValidation.ValueBoolean != null)
                            {
                                if (CheckGeneralValidation.Operation.Name == "==")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.BooleanValue == CheckGeneralValidation.ValueBoolean))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Equal to {CheckGeneralValidation.ValueBoolean}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "!=")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.BooleanValue != CheckGeneralValidation.ValueBoolean))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must not be Equal to {CheckGeneralValidation.ValueBoolean}");
                                }
                            }
                        }
                        else
                        {
                            return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute Must Have a Value and Can't be Null or Empty");
                        }
                    }

                    //
                    // Dependency..
                    //

                    List<IGrouping<int, Row_Rule>> Rules = _DbContext.Row_Rules
                        .Include(x => x.Rule).Include(x => x.Rule.Operation)
                        .Include(x => x.Rule.StaticAttribute).Include(x => x.Rule.DynamicAttribute)
                        .Include(x => x.Rule.NewDynamicAttribute)
                        .AsEnumerable().Where(x => x.Rule.NewDynamicAttributeId == DynamicAttribute.Id)
                        .GroupBy(x => x.RowId).ToList();

                    int CountOfSuccedRules = 0;

                    foreach (IGrouping<int, Row_Rule> RuleInOneRow in Rules)
                    {
                        foreach (Row_Rule Row_Rule in RuleInOneRow)
                        {
                            if (Row_Rule.Rule.StaticAttributeId != null)
                            {
                                string? ValueFromAddObject = PatientMedicalInformationViewModel.GetType().GetProperties()
                                    .FirstOrDefault(x => StaticAttributes.Any(y => y.Id == Row_Rule.Rule.StaticAttributeId) ?
                                        x.Name.ToLower() == StaticAttributes.FirstOrDefault(y => y.Id == Row_Rule.Rule.StaticAttributeId).Key.ToLower() : false)
                                    .GetValue(PatientMedicalInformationViewModel, null).ToString();

                                if (!string.IsNullOrEmpty(ValueFromAddObject))
                                {
                                    if (!string.IsNullOrEmpty(Row_Rule.Rule.OperationValueString))
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (ValueFromAddObject.ToLower() == Row_Rule.Rule.OperationValueString.ToLower())
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (ValueFromAddObject.ToLower() != Row_Rule.Rule.OperationValueString.ToLower())
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (Row_Rule.Rule.OperationValueDouble != null)
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (double.Parse(ValueFromAddObject) == Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (double.Parse(ValueFromAddObject) != Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<")
                                        {
                                            if (double.Parse(ValueFromAddObject) < Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<=")
                                        {
                                            if (double.Parse(ValueFromAddObject) <= Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">")
                                        {
                                            if (double.Parse(ValueFromAddObject) > Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">=")
                                        {
                                            if (double.Parse(ValueFromAddObject) >= Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (Row_Rule.Rule.OperationValueDateTime != null)
                                    {
                                        DateOnly ValueFromAddObjectDateTime = DateOnly.FromDateTime(DateTime.Parse(ValueFromAddObject));
                                        DateOnly OperationValueDateTime = DateOnly.FromDateTime(Row_Rule.Rule.OperationValueDateTime.Value);

                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (ValueFromAddObjectDateTime == OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (ValueFromAddObjectDateTime != OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<")
                                        {
                                            if (ValueFromAddObjectDateTime < OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<=")
                                        {
                                            if (ValueFromAddObjectDateTime <= OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">")
                                        {
                                            if (ValueFromAddObjectDateTime > OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">=")
                                        {
                                            if (ValueFromAddObjectDateTime >= OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (Row_Rule.Rule.OperationValueBoolean != null)
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (bool.Parse(ValueFromAddObject) == Row_Rule.Rule.OperationValueBoolean)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (bool.Parse(ValueFromAddObject) != Row_Rule.Rule.OperationValueBoolean)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                }
                                else
                                {
                                    return new ApiResponse(false, $"{Row_Rule.Rule.StaticAttribute.Key} Attribute Must Have a Value and Can't be Null or Empty");
                                }
                            }
                            else
                            {
                                bool CheckIfDynamicAttributeHasAValue = PatientMedicalInformationViewModel.DynamicAttributesValues
                                    .Any(x => x.Id == Row_Rule.Rule.DynamicAttributeId);

                                if (CheckIfDynamicAttributeHasAValue)
                                {
                                    AddDynamicAttributeValue? AddObject = PatientMedicalInformationViewModel.DynamicAttributesValues
                                        .FirstOrDefault(x => x.Id == Row_Rule.Rule.DynamicAttributeId);

                                    if (!string.IsNullOrEmpty(AddObject.StringValue) && !string.IsNullOrEmpty(Row_Rule.Rule.OperationValueString))
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (AddObject.StringValue.ToLower() == Row_Rule.Rule.OperationValueString.ToLower())
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (AddObject.StringValue.ToLower() != Row_Rule.Rule.OperationValueString.ToLower())
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (AddObject.NumberValue != null && Row_Rule.Rule.OperationValueDouble != null)
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (AddObject.NumberValue == Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (AddObject.NumberValue != Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<")
                                        {
                                            if (AddObject.NumberValue < Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<=")
                                        {
                                            if (AddObject.NumberValue <= Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">")
                                        {
                                            if (AddObject.NumberValue > Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">=")
                                        {
                                            if (AddObject.NumberValue >= Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (AddObject.DateTimeValue != null)
                                    {
                                        DateOnly DateTimeValue = DateOnly.FromDateTime(AddObject.DateTimeValue.Value);
                                        DateOnly OperationValueDateTime = DateOnly.FromDateTime(Row_Rule.Rule.OperationValueDateTime.Value);

                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (DateTimeValue == OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (DateTimeValue != OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<")
                                        {
                                            if (DateTimeValue < OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<=")
                                        {
                                            if (DateTimeValue <= OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">")
                                        {
                                            if (DateTimeValue > OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">=")
                                        {
                                            if (DateTimeValue >= OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (AddObject.BooleanValue != null)
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (AddObject.BooleanValue == Row_Rule.Rule.OperationValueBoolean)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (AddObject.BooleanValue != Row_Rule.Rule.OperationValueBoolean)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                }
                                else
                                {
                                    return new ApiResponse(false, $"{Row_Rule.Rule.DynamicAttribute.Key} Attribute Must Have a Value and Can't be Null or Empty");
                                }
                            }
                        }

                        if (CountOfSuccedRules == RuleInOneRow.Count())
                        {
                            Dependency? Dependency = _DbContext.Dependencies
                                .Include(x => x.Operation)
                                .FirstOrDefault(x => x.DynamicAttributeId == DynamicAttribute.Id);

                            if (Dependency != null)
                            {
                                AddDynamicAttributeValue? CheckIfDynamicAttributeHasAValue = PatientMedicalInformationViewModel.DynamicAttributesValues
                                    .FirstOrDefault(x => x.Id == DynamicAttribute.Id);

                                if (CheckIfDynamicAttributeHasAValue != null)
                                {
                                    if (!string.IsNullOrEmpty(CheckIfDynamicAttributeHasAValue.StringValue))
                                    {
                                        if (Dependency.Operation.Name == "==")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.StringValue.ToLower() == Dependency.ValueString.ToLower()))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Equal to {Dependency.ValueString}");
                                        }
                                        else if (Dependency.Operation.Name == "!=")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.StringValue.ToLower() != Dependency.ValueString.ToLower()))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be not Equal to {Dependency.ValueString}");
                                        }
                                    }
                                    else if (CheckIfDynamicAttributeHasAValue.NumberValue != null)
                                    {
                                        if (Dependency.Operation.Name == "==")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue == Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Equal to {Dependency.ValueDouble}");
                                        }
                                        else if (Dependency.Operation.Name == "!=")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue != Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be not Equal to {Dependency.ValueDouble}");
                                        }
                                        else if (Dependency.Operation.Name == ">")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue > Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Bigger Than {Dependency.ValueDouble}");
                                        }
                                        else if (Dependency.Operation.Name == ">=")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue >= Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Bigger Than or Equal to {Dependency.ValueDouble}");
                                        }
                                        else if (Dependency.Operation.Name == "<")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue < Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Smaller Than {Dependency.ValueDouble}");
                                        }
                                        else if (Dependency.Operation.Name == ">=")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue <= Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Smaller Than or Equal to {Dependency.ValueDouble}");
                                        }
                                    }
                                    else if (CheckIfDynamicAttributeHasAValue.DateTimeValue != null)
                                    {
                                        DateOnly DateTimeValue = DateOnly.FromDateTime(CheckIfDynamicAttributeHasAValue.DateTimeValue.Value);
                                        DateOnly ValueDateTime = DateOnly.FromDateTime(Dependency.ValueDateTime.Value);

                                        if (Dependency.Operation.Name == "==")
                                        {
                                            if (!(DateTimeValue == ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Equal to {ValueDateTime}");
                                        }
                                        else if (Dependency.Operation.Name == "!=")
                                        {
                                            if (!(DateTimeValue != ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be not Equal to {ValueDateTime}");
                                        }
                                        else if (Dependency.Operation.Name == ">")
                                        {
                                            if (!(DateTimeValue > ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Bigger Than {ValueDateTime}");
                                        }
                                        else if (Dependency.Operation.Name == ">=")
                                        {
                                            if (!(DateTimeValue >= ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Bigger Than or Equal to {ValueDateTime}");
                                        }
                                        else if (Dependency.Operation.Name == "<")
                                        {
                                            if (!(DateTimeValue < ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Smaller Than {ValueDateTime}");
                                        }
                                        else if (Dependency.Operation.Name == ">=")
                                        {
                                            if (!(DateTimeValue <= ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Smaller Than or Equal to {ValueDateTime}");
                                        }
                                    }
                                    else if (CheckIfDynamicAttributeHasAValue.BooleanValue != null)
                                    {
                                        if (Dependency.Operation.Name == "==")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.BooleanValue == Dependency.ValueBoolean))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Equal to {Dependency.ValueBoolean}");
                                        }
                                        else if (Dependency.Operation.Name == "!=")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.BooleanValue != Dependency.ValueBoolean))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be not Equal to {Dependency.ValueBoolean}");
                                        }
                                    }
                                }
                                else
                                {
                                    return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute Must Have a Value and Can't be Null or Empty");
                                }
                            }
                        }
                    }

                    foreach (AddDynamicAttributeValue DynamicAttributeValueForAdd in PatientMedicalInformationViewModel.DynamicAttributesValues)
                    {
                        Dynamic_Attribute_Value NewDynamicAttributeValueEntity = new Dynamic_Attribute_Value()
                        {
                            DynamicAttributeId = DynamicAttributeValueForAdd.Id,
                            IsDeleted = false,
                            PatientId = Patient.Id,
                            Disable = false,
                            ValueBoolean = DynamicAttributeValueForAdd.BooleanValue,
                            ValueDateTime = DynamicAttributeValueForAdd.DateTimeValue,
                            ValueDouble = DynamicAttributeValueForAdd.NumberValue,
                            ValueString = DynamicAttributeValueForAdd.StringValue
                        };

                        _DbContext.Dynamic_Attribute_Values.Add(NewDynamicAttributeValueEntity);
                        _DbContext.SaveChanges();
                    }
                }

                transaction.Complete();

                return new ApiResponse(true, "Succeed");
            }
        }
        public ApiResponse EditPatientMedicalInfo(EditMedical_InformationsViewModel PatientMedicalInformationViewModel)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                Patient? Patient = _DbContext.Patients
                    .FirstOrDefault(x => x.UserId == PatientMedicalInformationViewModel.PatientId);

                Medical_Information? PatientMedical_InformationEntity = _DbContext.Medical_Informations
                    .FirstOrDefault(x => x.PatientId == PatientMedicalInformationViewModel.PatientId);

                if (PatientMedical_InformationEntity == null)
                    return new ApiResponse(false, $"No Medical Information " +
                        $"Found For This Patient Id: ({PatientMedicalInformationViewModel.PatientId})");

                PatientMedical_InformationEntity.Height = PatientMedicalInformationViewModel.Height;
                PatientMedical_InformationEntity.BGroup = PatientMedicalInformationViewModel.BGroup;
                PatientMedical_InformationEntity.Pulse = PatientMedicalInformationViewModel.Pulse;
                PatientMedical_InformationEntity.Weight = PatientMedicalInformationViewModel.Weight;
                PatientMedical_InformationEntity.BPressure = PatientMedicalInformationViewModel.BPressure;
                PatientMedical_InformationEntity.Respiration = PatientMedicalInformationViewModel.Respiration;
                PatientMedical_InformationEntity.Diet = PatientMedicalInformationViewModel.Diet;

                _DbContext.SaveChanges();

                List<Dynamic_Attribute> DynamicAttributes = _DbContext.Dynamic_Attributes
                       .Where(x => !x.Disable).ToList();

                List<Static_Attribute> StaticAttributes = _DbContext.Static_Attributes.Where(x => x.Enable).ToList();

                foreach (Dynamic_Attribute DynamicAttribute in DynamicAttributes)
                {
                    //
                    // General Validation..
                    //

                    Validation? CheckGeneralValidation = _DbContext.Validations
                        .Include(x => x.Operation)
                        .FirstOrDefault(x => x.DynamicAttributeId == DynamicAttribute.Id);

                    if (CheckGeneralValidation != null)
                    {
                        AddDynamicAttributeValue? CheckIfDynamicAttributeHasAValue = PatientMedicalInformationViewModel.DynamicAttributesValues
                            .FirstOrDefault(x => x.Id == DynamicAttribute.Id);

                        if (CheckIfDynamicAttributeHasAValue != null)
                        {
                            if (!string.IsNullOrEmpty(CheckGeneralValidation.ValueString))
                            {
                                if (CheckGeneralValidation.Operation.Name == "==")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.StringValue.ToLower() == CheckGeneralValidation.ValueString.ToLower()))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Equal to {CheckGeneralValidation.ValueString}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "!=")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.StringValue.ToLower() != CheckGeneralValidation.ValueString.ToLower()))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must not be Equal to {CheckGeneralValidation.ValueString}");
                                }
                            }
                            else if (CheckGeneralValidation.ValueDouble != null)
                            {
                                if (CheckGeneralValidation.Operation.Name == "==")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.NumberValue == CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Equal to {CheckGeneralValidation.ValueDouble}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "!=")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.NumberValue != CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must not be Equal to {CheckGeneralValidation.ValueDouble}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == ">")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.NumberValue > CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Bigger Than {CheckGeneralValidation.ValueDouble}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == ">=")
                                {
                                    if (!(CheckGeneralValidation.ValueDouble > CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Bigger Than or Equal to {CheckGeneralValidation.ValueDouble}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "<")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.NumberValue < CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Smaller Than {CheckGeneralValidation.ValueDouble}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "<=")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.NumberValue <= CheckGeneralValidation.ValueDouble))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Smaller Than or Equal to {CheckGeneralValidation.ValueDouble}");
                                }
                            }
                            else if (CheckGeneralValidation.ValueDateTime != null)
                            {
                                DateOnly DateTimeValue = DateOnly.FromDateTime(CheckIfDynamicAttributeHasAValue.DateTimeValue.Value);
                                DateOnly ValueDateTime = DateOnly.FromDateTime(CheckGeneralValidation.ValueDateTime.Value);

                                if (CheckGeneralValidation.Operation.Name == "==")
                                {
                                    if (!(DateTimeValue == ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Equal to {ValueDateTime}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "!=")
                                {
                                    if (!(DateTimeValue != ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must not be Equal to {ValueDateTime}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == ">")
                                {
                                    if (!(DateTimeValue > ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Bigger Than {ValueDateTime}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == ">=")
                                {
                                    if (!(ValueDateTime > ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Bigger Than or Equal to {ValueDateTime}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "<")
                                {
                                    if (!(DateTimeValue < ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Smaller Than {ValueDateTime}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "<=")
                                {
                                    if (!(DateTimeValue <= ValueDateTime))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Smaller Than or Equal to {ValueDateTime}");
                                }
                            }
                            else if (CheckGeneralValidation.ValueBoolean != null)
                            {
                                if (CheckGeneralValidation.Operation.Name == "==")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.BooleanValue == CheckGeneralValidation.ValueBoolean))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must be Equal to {CheckGeneralValidation.ValueBoolean}");
                                }
                                else if (CheckGeneralValidation.Operation.Name == "!=")
                                {
                                    if (!(CheckIfDynamicAttributeHasAValue.BooleanValue != CheckGeneralValidation.ValueBoolean))
                                        return new ApiResponse(false, $"{DynamicAttribute.Key} Column's Value Must not be Equal to {CheckGeneralValidation.ValueBoolean}");
                                }
                            }
                        }
                        else
                        {
                            return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute Must Have a Value and Can't be Null or Empty");
                        }
                    }

                    //
                    // Dependency..
                    //

                    List<IGrouping<int, Row_Rule>> Rules = _DbContext.Row_Rules
                        .Include(x => x.Rule).Include(x => x.Rule.Operation)
                        .Include(x => x.Rule.StaticAttribute).Include(x => x.Rule.DynamicAttribute)
                        .Include(x => x.Rule.NewDynamicAttribute)
                        .AsEnumerable().Where(x => x.Rule.NewDynamicAttributeId == DynamicAttribute.Id)
                        .GroupBy(x => x.RowId).ToList();

                    int CountOfSuccedRules = 0;

                    foreach (IGrouping<int, Row_Rule> RuleInOneRow in Rules)
                    {
                        foreach (Row_Rule Row_Rule in RuleInOneRow)
                        {
                            if (Row_Rule.Rule.StaticAttributeId != null)
                            {
                                string? ValueFromAddObject = PatientMedicalInformationViewModel.GetType().GetProperties()
                                    .FirstOrDefault(x => StaticAttributes.Any(y => y.Id == Row_Rule.Rule.StaticAttributeId) ?
                                        x.Name.ToLower() == StaticAttributes.FirstOrDefault(y => y.Id == Row_Rule.Rule.StaticAttributeId).Key.ToLower() : false)
                                    .GetValue(PatientMedicalInformationViewModel, null).ToString();

                                if (!string.IsNullOrEmpty(ValueFromAddObject))
                                {
                                    if (!string.IsNullOrEmpty(Row_Rule.Rule.OperationValueString))
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (ValueFromAddObject.ToLower() == Row_Rule.Rule.OperationValueString.ToLower())
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (ValueFromAddObject.ToLower() != Row_Rule.Rule.OperationValueString.ToLower())
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (Row_Rule.Rule.OperationValueDouble != null)
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (double.Parse(ValueFromAddObject) == Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (double.Parse(ValueFromAddObject) != Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<")
                                        {
                                            if (double.Parse(ValueFromAddObject) < Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<=")
                                        {
                                            if (double.Parse(ValueFromAddObject) <= Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">")
                                        {
                                            if (double.Parse(ValueFromAddObject) > Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">=")
                                        {
                                            if (double.Parse(ValueFromAddObject) >= Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (Row_Rule.Rule.OperationValueDateTime != null)
                                    {
                                        DateOnly ValueFromAddObjectDateTime = DateOnly.FromDateTime(DateTime.Parse(ValueFromAddObject));
                                        DateOnly OperationValueDateTime = DateOnly.FromDateTime(Row_Rule.Rule.OperationValueDateTime.Value);

                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (ValueFromAddObjectDateTime == OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (ValueFromAddObjectDateTime != OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<")
                                        {
                                            if (ValueFromAddObjectDateTime < OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<=")
                                        {
                                            if (ValueFromAddObjectDateTime <= OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">")
                                        {
                                            if (ValueFromAddObjectDateTime > OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">=")
                                        {
                                            if (ValueFromAddObjectDateTime >= OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (Row_Rule.Rule.OperationValueBoolean != null)
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (bool.Parse(ValueFromAddObject) == Row_Rule.Rule.OperationValueBoolean)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (bool.Parse(ValueFromAddObject) != Row_Rule.Rule.OperationValueBoolean)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                }
                                else
                                {
                                    return new ApiResponse(false, $"{Row_Rule.Rule.StaticAttribute.Key} Attribute Must Have a Value and Can't be Null or Empty");
                                }
                            }
                            else
                            {
                                bool CheckIfDynamicAttributeHasAValue = PatientMedicalInformationViewModel.DynamicAttributesValues
                                    .Any(x => x.Id == Row_Rule.Rule.DynamicAttributeId);

                                if (CheckIfDynamicAttributeHasAValue)
                                {
                                    AddDynamicAttributeValue? AddObject = PatientMedicalInformationViewModel.DynamicAttributesValues
                                        .FirstOrDefault(x => x.Id == Row_Rule.Rule.DynamicAttributeId);

                                    if (!string.IsNullOrEmpty(AddObject.StringValue) && !string.IsNullOrEmpty(Row_Rule.Rule.OperationValueString))
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (AddObject.StringValue.ToLower() == Row_Rule.Rule.OperationValueString.ToLower())
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (AddObject.StringValue.ToLower() != Row_Rule.Rule.OperationValueString.ToLower())
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (AddObject.NumberValue != null && Row_Rule.Rule.OperationValueDouble != null)
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (AddObject.NumberValue == Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (AddObject.NumberValue != Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<")
                                        {
                                            if (AddObject.NumberValue < Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<=")
                                        {
                                            if (AddObject.NumberValue <= Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">")
                                        {
                                            if (AddObject.NumberValue > Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">=")
                                        {
                                            if (AddObject.NumberValue >= Row_Rule.Rule.OperationValueDouble)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (AddObject.DateTimeValue != null)
                                    {
                                        DateOnly DateTimeValue = DateOnly.FromDateTime(AddObject.DateTimeValue.Value);
                                        DateOnly OperationValueDateTime = DateOnly.FromDateTime(Row_Rule.Rule.OperationValueDateTime.Value);

                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (DateTimeValue == OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (DateTimeValue != OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<")
                                        {
                                            if (DateTimeValue < OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "<=")
                                        {
                                            if (DateTimeValue <= OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">")
                                        {
                                            if (DateTimeValue > OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == ">=")
                                        {
                                            if (DateTimeValue >= OperationValueDateTime)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                    else if (AddObject.BooleanValue != null)
                                    {
                                        if (Row_Rule.Rule.Operation.Name == "==")
                                        {
                                            if (AddObject.BooleanValue == Row_Rule.Rule.OperationValueBoolean)
                                                CountOfSuccedRules++;
                                        }
                                        else if (Row_Rule.Rule.Operation.Name == "!=")
                                        {
                                            if (AddObject.BooleanValue != Row_Rule.Rule.OperationValueBoolean)
                                                CountOfSuccedRules++;
                                        }
                                    }
                                }
                                else
                                {
                                    return new ApiResponse(false, $"{Row_Rule.Rule.DynamicAttribute.Key} Attribute Must Have a Value and Can't be Null or Empty");
                                }
                            }
                        }

                        if (CountOfSuccedRules == RuleInOneRow.Count())
                        {
                            Dependency? Dependency = _DbContext.Dependencies
                                .Include(x => x.Operation)
                                .FirstOrDefault(x => x.DynamicAttributeId == DynamicAttribute.Id);

                            if (Dependency != null)
                            {
                                AddDynamicAttributeValue? CheckIfDynamicAttributeHasAValue = PatientMedicalInformationViewModel.DynamicAttributesValues
                                    .FirstOrDefault(x => x.Id == DynamicAttribute.Id);

                                if (CheckIfDynamicAttributeHasAValue != null)
                                {
                                    if (!string.IsNullOrEmpty(CheckIfDynamicAttributeHasAValue.StringValue))
                                    {
                                        if (Dependency.Operation.Name == "==")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.StringValue.ToLower() == Dependency.ValueString.ToLower()))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Equal to {Dependency.ValueString}");
                                        }
                                        else if (Dependency.Operation.Name == "!=")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.StringValue.ToLower() != Dependency.ValueString.ToLower()))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be not Equal to {Dependency.ValueString}");
                                        }
                                    }
                                    else if (CheckIfDynamicAttributeHasAValue.NumberValue != null)
                                    {
                                        if (Dependency.Operation.Name == "==")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue == Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Equal to {Dependency.ValueDouble}");
                                        }
                                        else if (Dependency.Operation.Name == "!=")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue != Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be not Equal to {Dependency.ValueDouble}");
                                        }
                                        else if (Dependency.Operation.Name == ">")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue > Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Bigger Than {Dependency.ValueDouble}");
                                        }
                                        else if (Dependency.Operation.Name == ">=")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue >= Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Bigger Than or Equal to {Dependency.ValueDouble}");
                                        }
                                        else if (Dependency.Operation.Name == "<")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue < Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Smaller Than {Dependency.ValueDouble}");
                                        }
                                        else if (Dependency.Operation.Name == ">=")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.NumberValue <= Dependency.ValueDouble))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Smaller Than or Equal to {Dependency.ValueDouble}");
                                        }
                                    }
                                    else if (CheckIfDynamicAttributeHasAValue.DateTimeValue != null)
                                    {
                                        DateOnly DateTimeValue = DateOnly.FromDateTime(CheckIfDynamicAttributeHasAValue.DateTimeValue.Value);
                                        DateOnly ValueDateTime = DateOnly.FromDateTime(Dependency.ValueDateTime.Value);

                                        if (Dependency.Operation.Name == "==")
                                        {
                                            if (!(DateTimeValue == ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Equal to {ValueDateTime}");
                                        }
                                        else if (Dependency.Operation.Name == "!=")
                                        {
                                            if (!(DateTimeValue != ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be not Equal to {ValueDateTime}");
                                        }
                                        else if (Dependency.Operation.Name == ">")
                                        {
                                            if (!(DateTimeValue > ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Bigger Than {ValueDateTime}");
                                        }
                                        else if (Dependency.Operation.Name == ">=")
                                        {
                                            if (!(DateTimeValue >= ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Bigger Than or Equal to {ValueDateTime}");
                                        }
                                        else if (Dependency.Operation.Name == "<")
                                        {
                                            if (!(DateTimeValue < ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Smaller Than {ValueDateTime}");
                                        }
                                        else if (Dependency.Operation.Name == ">=")
                                        {
                                            if (!(DateTimeValue <= ValueDateTime))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Smaller Than or Equal to {ValueDateTime}");
                                        }
                                    }
                                    else if (CheckIfDynamicAttributeHasAValue.BooleanValue != null)
                                    {
                                        if (Dependency.Operation.Name == "==")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.BooleanValue == Dependency.ValueBoolean))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be Equal to {Dependency.ValueBoolean}");
                                        }
                                        else if (Dependency.Operation.Name == "!=")
                                        {
                                            if (!(CheckIfDynamicAttributeHasAValue.BooleanValue != Dependency.ValueBoolean))
                                                return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute's Value Must be not Equal to {Dependency.ValueBoolean}");
                                        }
                                    }
                                }
                                else
                                {
                                    return new ApiResponse(false, $"{DynamicAttribute.Key} Attribute Must Have a Value and Can't be Null or Empty");
                                }
                            }
                        }
                    }

                    foreach (AddDynamicAttributeValue DynamicAttributeValueForAdd in PatientMedicalInformationViewModel.DynamicAttributesValues)
                    {
                        Dynamic_Attribute_Value? CheckDynamic_Attribute_ValueIfAlreadyExist = _DbContext.Dynamic_Attribute_Values
                            .FirstOrDefault(x => x.PatientId == Patient.Id && x.DynamicAttributeId == DynamicAttributeValueForAdd.Id);

                        if (CheckDynamic_Attribute_ValueIfAlreadyExist == null)
                        {
                            Dynamic_Attribute_Value NewDynamicAttributeValueEntity = new Dynamic_Attribute_Value()
                            {
                                DynamicAttributeId = DynamicAttributeValueForAdd.Id,
                                IsDeleted = false,
                                PatientId = Patient.Id,
                                Disable = false,
                                ValueBoolean = DynamicAttributeValueForAdd.BooleanValue,
                                ValueDateTime = DynamicAttributeValueForAdd.DateTimeValue,
                                ValueDouble = DynamicAttributeValueForAdd.NumberValue,
                                ValueString = DynamicAttributeValueForAdd.StringValue
                            };

                            _DbContext.Dynamic_Attribute_Values.Add(NewDynamicAttributeValueEntity);
                            _DbContext.SaveChanges();
                        }
                        else
                        {
                            CheckDynamic_Attribute_ValueIfAlreadyExist.ValueDouble = DynamicAttributeValueForAdd.NumberValue;
                            CheckDynamic_Attribute_ValueIfAlreadyExist.ValueBoolean = DynamicAttributeValueForAdd.BooleanValue;
                            CheckDynamic_Attribute_ValueIfAlreadyExist.ValueString = DynamicAttributeValueForAdd.StringValue;
                            CheckDynamic_Attribute_ValueIfAlreadyExist.ValueDateTime = DynamicAttributeValueForAdd.DateTimeValue;

                            _DbContext.SaveChanges();
                        }
                    }
                }

                transaction.Complete();

                return new ApiResponse(true, "Succeed");
            }
        }
        public ApiResponse GetMedicalInformationByPatientId(int PatientId)
        {
            Medical_Information? Medical_InformationEntity = _DbContext.Medical_Informations
                .Include(x => x.Patient).FirstOrDefault(x => x.PatientId == PatientId);

            if (Medical_InformationEntity == null)
                return new ApiResponse(new Medical_InformationViewModel(), "Succeed");

            Medical_InformationViewModel PatientMedicalInformation = _Mapper.Map<Medical_InformationViewModel>(Medical_InformationEntity);

            return new ApiResponse(PatientMedicalInformation, "Succeed");
        }
        public ApiResponse GetPatientById(int PatientId)
        {
            Patient? PatientEntity = _DbContext.Patients.Include(x => x.User)
                .FirstOrDefault(x => x.UserId == PatientId);

            if (PatientEntity == null)
                return new ApiResponse(false, $"No Patient Found With This Id: ({PatientId})");

            PatientViewModel PatientViewModel = _Mapper.Map<PatientViewModel>(PatientEntity);

            PatientViewModel.VisitCount = _DbContext.Appointments
                .Where(x => x.PatientId == PatientViewModel.Id && x.Status == ((int)Constants.AppointmentStatus.complete).ToString()).Count();

            return new ApiResponse(PatientViewModel, "Succeed");
        }
        public ApiResponse GetAllDoctorsPatients(int UserId, ComplexFilter Filter)
        {
            Doctor? DoctorEntity = _DbContext.Doctors.FirstOrDefault(x => x.UserId == UserId);

            if (DoctorEntity == null)
                return new ApiResponse(false, $"No Doctor Found With This Id: ({UserId})");

            List<PatientViewModel> Patients = _Mapper.Map<List<PatientViewModel>>(_DbContext.Appointments
                .Include(x => x.Patient).Include(x => x.Patient.User)
                .Where(x => x.DoctorId == DoctorEntity.Id)
                .Select(x => x.Patient).ToList());

            foreach (PatientViewModel Patient in Patients)
            {
                Patient.VisitCount = _DbContext.Appointments
                    .Where(x => x.PatientId == Patient.Id && x.Status == ((int)Constants.AppointmentStatus.complete).ToString()).Count();
            }

            int Count = Patients.Count();

            if (!string.IsNullOrEmpty(Filter.Sort))
            {
                PropertyInfo? SortProperty = typeof(PatientViewModel).GetProperty(Filter.Sort);

                if (SortProperty != null && Filter.Order == "asc")
                    Patients = Patients.OrderBy(x => SortProperty.GetValue(x)).ToList();

                else if (SortProperty != null && Filter.Order == "desc")
                    Patients = Patients.OrderByDescending(x => SortProperty.GetValue(x)).ToList();

                Patients = Patients.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Patients, "Succeed", Count);
            }
            else
            {
                Patients = Patients.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Patients, "Succeed", Count);
            }
        }
    }
}
