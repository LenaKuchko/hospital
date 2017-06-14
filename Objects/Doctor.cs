using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Hospital.Objects
{
  public class Doctor
  {
    public int Id {get; set;}
    public string Name {get; set;}
    public string UserName {get; set;}
    public string Password {get; set;}
    public string Specialty {get; set;}

    public Doctor()
     {
       Id = 0;
       Name = null;
       UserName = null;
       Password = null;
       Specialty = null;
     }

    public Doctor(string name, string userName, string password, string specialty, int id = 0)
    {
      Id = id;
      Name = name;
      UserName = userName;
      Password = password;
      Specialty = specialty;
    }

    public override bool Equals(System.Object otherDoctor)
    {
      if (!(otherDoctor is Doctor))
      {
        return false;
      }
      else
      {
        Doctor newDoctor = (Doctor) otherDoctor;
        return (this.Id == newDoctor.Id &&
                this.Name == newDoctor.Name &&
                this.UserName == newDoctor.UserName &&
                this.Password == newDoctor.Password &&
                this.Specialty == newDoctor.Specialty);
      }
    }

    public static List<Doctor>GetAll()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM doctors;", DB.GetConnection());
      SqlDataReader rdr = cmd.ExecuteReader();

      List<Doctor> allDoctors = new List<Doctor>{};
      while (rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string userName = rdr.GetString(2);
        string password = rdr.GetString(3);
        string specialty = rdr.GetString(4);

        Doctor newDoctor = new Doctor(name, userName, password, specialty, id);
        allDoctors.Add(newDoctor);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return allDoctors;
    }

    public void Save()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("INSERT INTO doctors (name, username, password, specialty) OUTPUT INSERTED.id VALUES (@DoctorName, @DoctorUserName, @DoctorPassword, @DoctorSpecialty)", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@DoctorName", this.Name));
      cmd.Parameters.Add(new SqlParameter("@DoctorUserName", this.UserName));
      cmd.Parameters.Add(new SqlParameter("@DoctorPassword", this.Password));
      cmd.Parameters.Add(new SqlParameter("@DoctorSpecialty", this.Specialty));

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

    public static Doctor Find(int searchId)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM doctors WHERE id = @DoctorId;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@DoctorId", searchId));

      SqlDataReader rdr = cmd.ExecuteReader();

      Doctor foundDoctor = new Doctor();
      while (rdr.Read())
      {
        foundDoctor.Id = rdr.GetInt32(0);
        foundDoctor.Name = rdr.GetString(1);
        foundDoctor.UserName = rdr.GetString(2);
        foundDoctor.Password = rdr.GetString(3);
        foundDoctor.Specialty = rdr.GetString(4);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return foundDoctor;
    }

    public static void DeleteAll()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM doctors;", DB.GetConnection());
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }
  }
}
