using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class deleteProfile
    {
        dbServices ds = new dbServices();

        public async Task<responseData> DeleteProfile(requestData req)
        {
            responseData resData = new responseData();

            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", req.addInfo["UserId"].ToString())
                };

                var deleteSql = @"DELETE FROM pc_student.Ak_register WHERE UserId = @UserId";

                var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);

                if (rowsAffected > 0)
                {
                    resData.eventID = req.eventID;
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Profile deleted successfully";
                }
                else
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No records deleted";
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 404;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }

            return resData;
        }
    }
}
