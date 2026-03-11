using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Business_Logic_Layer.Service.util
{
    public static class PasswordHelper
    {
        // دالة لتشفير كلمة المرور
        public static string HashPassword(string password)
        {
            // 12 هو عدد دورات التشفير (Work Factor)، كلما زاد زاد الأمان وزاد الوقت
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        // دالة للتحقق من كلمة المرور عند تسجيل الدخول
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
