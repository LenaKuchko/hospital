using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Hospital.Objects
{
  public class Patient
  {
    public int Id {get; set;}
    public string Name {get; set;}
    public string UserName {get; set;}
    public string Password {get; set;}
    public DateTime DOB {get; set;}


    public Patient(string name, string userName, string password, DateTime dob, int id = 0)
    {
      Id = id;
      Name = name;
      UserName = userName;
      Password = password;
      DOB = dob;
    }

    public override bool Equals(System.Object otherPatient)
    {
      if (!(otherPatient is Patient))
      {
        return false;
      }
      else
      {
        Patient newPatient = (Patient) otherPatient;
        return (this.Id == newPatient.Id &&
                this.Name == newPatient.Name &&
                this.UserName == newPatient.UserName &&
                this.Password == newPatient.Password &&
                this.DOB == newPatient.DOB);
      }
    }

    public static List<Patient>GetAll()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM patients;", DB.GetConnection());
      SqlDataReader rdr = cmd.ExecuteReader();

      List<Patient> allPatients = new List<Patient>{};
      while (rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string userName = rdr.GetString(2);
        string password = rdr.GetString(3);
        DateTime dob = rdr.GetDateTime(4);

        Patient newPatient = new Patient(name, userName, password, dob, id);
        allPatients.Add(newPatient);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return allPatients;
    }

    public void Save()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("INSERT INTO patients (name, username, password, dob) OUTPUT INSERTED.id VALUES (@PatientName, @PatientUserName, @PatientPassword, @PatientDOB)", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@PatientName", this.Name));
      cmd.Parameters.Add(new SqlParameter("@PatientUserName", this.UserName));
      cmd.Parameters.Add(new SqlParameter("@PatientPassword", this.Password));
      cmd.Parameters.Add(new SqlParameter("@PatientDOB", this.DOB));

      SqlDataReader rdr = cmd.ExecuteReader();
      while(rdr.Read())
      {
        this.Id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();
    }

    public static void DeleteAll()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM patients;", DB.GetConnection());
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }
  }
}
