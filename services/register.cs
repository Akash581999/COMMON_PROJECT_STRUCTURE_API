using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class register
    {
        dbServices ds = new dbServices();
        public async Task<responseData> Register(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.rData["rMessage"] = "Registered successfully";

            try
            {
                MySqlParameter[] para = new MySqlParameter[] {
            // new MySqlParameter("@UserId", req.addInfo["UserId"].ToString()),
            // new MySqlParameter("@ROLE_ID", req.addInfo["ROLE_ID"].ToString()),
            new MySqlParameter("@FirstName", req.addInfo["FirstName"].ToString()),
            new MySqlParameter("@LastName", req.addInfo["LastName"].ToString()),
            new MySqlParameter("@EMAIL_ID", req.addInfo["EMAIL_ID"].ToString()),
            new MySqlParameter("@MOBILE_NO", req.addInfo["MOBILE_NO"].ToString()),
            new MySqlParameter("@UserPassword", req.addInfo["UserPassword"].ToString())
        };
                var checkSql = $"SELECT * FROM pc_student.Ak_register WHERE MOBILE_NO=@MOBILE_NO OR EMAIL_ID=@EMAIL_ID;";
                var checkResult = ds.executeSQL(checkSql, para);
                if (checkResult[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "User already registered, Try Login in!!";
                }
                else
                {
                    var insertSql = $"INSERT INTO pc_student.Ak_register (UserPassword, FirstName, LastName, EMAIL_ID, MOBILE_NO) VALUES(@UserPassword, @FirstName, @LastName, @EMAIL_ID, @MOBILE_NO);";
                    var insertId = ds.ExecuteInsertAndGetLastId(insertSql, para);

                    if (insertId != null)
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rMessage"] = "Account created successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
    }
}