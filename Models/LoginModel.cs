using Microsoft.Data.SqlClient;

namespace AuthProject.Models
{
    public class LoginModel
    {
        private string conectstring = "Data Source=localhost;Initial Catalog=db_mawdyasistencia;Persist Security Info=True;User ID=sa;Password=@Asistencia.10;Encrypt=True;Trust Server Certificate=True";
        public string Email { get; set; }
        public string PassWord { get; set; }
        public bool KeepLoggedIn { get; set; }

        public bool ValidarUsuario(String Usuario, String Password)
        {

            using (SqlConnection con = new SqlConnection(conectstring))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM tbl_usuarios WHERE email = @Email AND h_password = @Password";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", Usuario);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // Si hay al menos un registro, las credenciales son válidas
                }
            }
        }
    }
}
