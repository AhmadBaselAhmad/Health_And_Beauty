﻿using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Allergies;
using GraduationProject.DataBase.ViewModels.Immunization;
using GraduationProject.DataBase.ViewModels.Medicine;
using GraduationProject.DataBase.ViewModels.Surgery;
using GraduationProject.Service.Interfaces;
using OfficeOpenXml;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace GraduationProject.Service.Services
{
    public class MedicalInformationService : IMedicalInformationService
    {
        private GraduationProjectDbContext _DbContext;
        private readonly IMapper _Mapper;
        public MedicalInformationService(GraduationProjectDbContext DbContext, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Mapper = Mapper;
        }
        public ApiResponse GetAllAlergies(IFormFile File)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ExcelPackageInstance = new ExcelPackage();

            CultureInfo CultureInfoInstance = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CultureInfoInstance.NumberFormat.NumberDecimalSeparator = ",";

            DataTable DataTableInstance = new DataTable();
            string FileConverter = FileConverterFuction(File);

            OleDbDataAdapter AllergiesInformation = new OleDbDataAdapter("SELECT * FROM [Allregies$]", FileConverter);
            DataSet Allergies = new DataSet();

            AllergiesInformation.Fill(Allergies, "ExcelTable");

            DataTable AllergiesEntities = Allergies.Tables["ExcelTable"];

            DataTableInstance.Clear();
            DataTableInstance.Columns.Clear();

            string[]? AllergiesColumnNames = AllergiesEntities.Columns.Cast<DataColumn>()
                .Select(x => x.ColumnName)
                .ToArray();

            DataRow firstRowAllergy = DataTableInstance.NewRow();
            int ColumnNumber = 0;

            foreach (string? AllergyColumnName in AllergiesColumnNames)
            {
                DataTableInstance.Columns.Add(AllergyColumnName);
                firstRowAllergy[ColumnNumber] = AllergyColumnName;
                ColumnNumber++;
            }

            int RowNumber = 0;
            DataTableInstance.Rows.Add(firstRowAllergy);

            List<AllergyViewModel> AllergiesOutPuts = new List<AllergyViewModel>();

            foreach (DataRow row in AllergiesEntities.Rows)
            {
                RowNumber++;

                AllergiesOutPuts.Add(new AllergyViewModel()
                {
                    Type = row["Type"].ToString(),
                    Description = row["Description"].ToString(),
                    AllergicTo = row["AllergicTo"].ToString()
                });
            }

            return new ApiResponse(AllergiesOutPuts, "Succeed", AllergiesOutPuts.Count());
        }
        public ApiResponse GetAllImmunizations(IFormFile File)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ExcelPackageInstance = new ExcelPackage();

            CultureInfo CultureInfoInstance = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CultureInfoInstance.NumberFormat.NumberDecimalSeparator = ",";

            DataTable DataTableInstance = new DataTable();
            string FileConverter = FileConverterFuction(File);

            OleDbDataAdapter ImmunizationsInformation = new OleDbDataAdapter("SELECT * FROM [Immunization$]", FileConverter);
            DataSet Immunizations = new DataSet();

            ImmunizationsInformation.Fill(Immunizations, "ExcelTable");

            DataTable ImmunizationsEntities = Immunizations.Tables["ExcelTable"];

            DataTableInstance.Clear();
            DataTableInstance.Columns.Clear();

            string[]? ImmunizationsColumnNames = ImmunizationsEntities.Columns.Cast<DataColumn>()
                .Select(x => x.ColumnName)
                .ToArray();

            DataRow firstRowImmunization = DataTableInstance.NewRow();
            int ColumnNumber = 0;

            foreach (string? ImmunizationColumnName in ImmunizationsColumnNames)
            {
                DataTableInstance.Columns.Add(ImmunizationColumnName);
                firstRowImmunization[ColumnNumber] = ImmunizationColumnName;
                ColumnNumber++;
            }

            int RowNumber = 0;
            DataTableInstance.Rows.Add(firstRowImmunization);

            List<ImmunizationViewModel> ImmunizationsOutPuts = new List<ImmunizationViewModel>();

            foreach (DataRow row in ImmunizationsEntities.Rows)
            {
                RowNumber++;

                ImmunizationsOutPuts.Add(new ImmunizationViewModel()
                {
                    Name = row["Name"].ToString(),
                    Description = row["Description"].ToString()
                });
            }

            return new ApiResponse(ImmunizationsOutPuts, "Succeed", ImmunizationsOutPuts.Count());
        }
        public ApiResponse GetAllMedicines(IFormFile File)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ExcelPackageInstance = new ExcelPackage();

            CultureInfo CultureInfoInstance = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CultureInfoInstance.NumberFormat.NumberDecimalSeparator = ",";

            DataTable DataTableInstance = new DataTable();
            string FileConverter = FileConverterFuction(File);

            OleDbDataAdapter MedicineInformation = new OleDbDataAdapter("SELECT * FROM [Medicine$]", FileConverter);
            DataSet Medicines = new DataSet();

            MedicineInformation.Fill(Medicines, "ExcelTable");

            DataTable MedicineEntities = Medicines.Tables["ExcelTable"];

            DataTableInstance.Clear();
            DataTableInstance.Columns.Clear();

            string[]? MedicinesColumnNames = MedicineEntities.Columns.Cast<DataColumn>()
                .Select(x => x.ColumnName)
                .ToArray();

            DataRow firstRowMedicine = DataTableInstance.NewRow();
            int ColumnNumber = 0;

            foreach (string? MedicineColumnName in MedicinesColumnNames)
            {
                DataTableInstance.Columns.Add(MedicineColumnName);
                firstRowMedicine[ColumnNumber] = MedicineColumnName;
                ColumnNumber++;
            }

            int RowNumber = 0;
            DataTableInstance.Rows.Add(firstRowMedicine);

            List<MedicineViewModel> MedicinesOutPuts = new List<MedicineViewModel>();

            foreach (DataRow row in MedicineEntities.Rows)
            {
                RowNumber++;

                MedicinesOutPuts.Add(new MedicineViewModel()
                {
                    Name = row["Name"].ToString(),
                    Description = row["Description"].ToString()
                });
            }

            return new ApiResponse(MedicinesOutPuts, "Succeed", MedicinesOutPuts.Count());
        }
        public ApiResponse GetAllSurgeries(IFormFile File)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ExcelPackageInstance = new ExcelPackage();

            CultureInfo CultureInfoInstance = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CultureInfoInstance.NumberFormat.NumberDecimalSeparator = ",";

            DataTable DataTableInstance = new DataTable();
            string FileConverter = FileConverterFuction(File);

            OleDbDataAdapter SurgeriesInformation = new OleDbDataAdapter("SELECT * FROM [Surgery$]", FileConverter);
            DataSet Surgeries = new DataSet();

            SurgeriesInformation.Fill(Surgeries, "ExcelTable");

            DataTable SurgeriesEntities = Surgeries.Tables["ExcelTable"];

            DataTableInstance.Clear();
            DataTableInstance.Columns.Clear();

            string[]? SurgeriesColumnNames = SurgeriesEntities.Columns.Cast<DataColumn>()
                .Select(x => x.ColumnName)
                .ToArray();

            DataRow firstRowSurgery = DataTableInstance.NewRow();
            int ColumnNumber = 0;

            foreach (string? SurgeryColumnName in SurgeriesColumnNames)
            {
                DataTableInstance.Columns.Add(SurgeryColumnName);
                firstRowSurgery[ColumnNumber] = SurgeryColumnName;
                ColumnNumber++;
            }

            int RowNumber = 0;
            DataTableInstance.Rows.Add(firstRowSurgery);

            List<SurgeryViewModel> SurgeriesOutPuts = new List<SurgeryViewModel>();

            foreach (DataRow row in SurgeriesEntities.Rows)
            {
                RowNumber++;

                SurgeriesOutPuts.Add(new SurgeryViewModel()
                {
                    Name = row["Name"].ToString(),
                    Description = row["Description"].ToString()
                });
            }

            return new ApiResponse(SurgeriesOutPuts, "Succeed", SurgeriesOutPuts.Count());
        }

        public string FileConverterFuction(IFormFile file)
        {
            string fileDirectory = Directory.GetCurrentDirectory();
            string DirectoryPath = null;
            string FilePath = null;
            DirectoryPath = Path.Combine(fileDirectory, "ExcelData");

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            if (file.Length > 0)
            {
                var FileName = file.FileName;
                FilePath = Path.Combine(DirectoryPath, $"{FileName}");

                using (Stream fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            string filePath = FilePath;
            FileInfo existingFile = new FileInfo(filePath);

            var connectionString = string.Empty;

            if (existingFile.FullName.EndsWith(".xls"))
            {
                connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", filePath);
            }
            else if (existingFile.FullName.EndsWith(".xlsx"))
            {
                connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", filePath);
            }

            return connectionString;
        }
    }
}
