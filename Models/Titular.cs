using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;


namespace MawdyAsistenciaApp.Models
{
    public class Titular
    {
        private string conectstring = "Data Source=localhost;Initial Catalog=db_mawdyasistencia;Persist Security Info=True;User ID=sa;Password=@Asistencia.10;Encrypt=True;Trust Server Certificate=True";

        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Motor { get; set; }
        public string Chasis { get; set; }
        public string Placa { get; set; }
        public int Año { get; set; }
        public string Color { get; set; }
        public string Direccion { get; set; }
        public string Miembro { get; set; }
        public int ContratoId { get; set; }
        public string NombreContrato { get; set; }
        public string Poliza { get; set; }
        public string Certificado { get; set; }
        public string DNI { get; set; }
        public char Sexo { get; set; }


        public List<Titular> BuscarTitulares(String parametroBusqueda)
        {
            List<Titular> resultadoTitulares = new List<Titular>();
            using (SqlConnection con = new SqlConnection(conectstring))
            {
                SqlDataReader reader = null;
                con.Open();

                SqlCommand cmd = new SqlCommand("sp_BuscarTitulares", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@parametro_busqueda", parametroBusqueda);
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    Titular titularConsulta = new Titular(); 
                    titularConsulta.NombreCompleto = reader["nombre_completo"].ToString();
                    titularConsulta.Direccion = reader["direccion"].ToString();
                    titularConsulta.Poliza = reader["poliza"].ToString();
                    titularConsulta.NombreContrato = reader["nombre_contrato"].ToString();
                    titularConsulta.Chasis = reader["chasis"].ToString();
                    titularConsulta.Motor = reader["motor"].ToString();
                    titularConsulta.DNI = reader["dni"].ToString();
                    titularConsulta.Placa = reader["placa"].ToString();

                    resultadoTitulares.Add(titularConsulta);

                }
            }

            return resultadoTitulares;
        }
    }
}
