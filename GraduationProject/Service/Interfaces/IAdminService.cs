﻿using GraduationProject.DataBase.Helpers;

namespace GraduationProject.Service.Interfaces
{
    public interface IAdminService
    {
        ApiResponse ChangeCurrentAdmin(int DoctorId);
    }
}
