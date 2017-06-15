using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Hospital.Objects
{
  public class Doctor
  {
    // static proerties to act as doctor password?
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

    public void AddPatient(Patient patientToAdd)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("INSERT INTO doctors_patients (doctor_id, patient_id) VALUES (@DoctorId, @PatientId);", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@DoctorId", this.Id));
      cmd.Parameters.Add(new SqlParameter("@PatientId", patientToAdd.Id));

      cmd.ExecuteNonQuery();

      DB.CloseConnection();
    }

    public List<Patient> GetPatients()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT patients.* FROM doctors JOIN doctors_patients ON (doctors.id = doctors_patients.doctor_id) JOIN patients ON (patients.id = doctors_patients.patient_id) WHERE doctor_id = @DoctorId;", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@DoctorId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Patient> patients = new List<Patient>{};
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string userName = rdr.GetString(2);
        string password = rdr.GetString(3);
        DateTime dob = rdr.GetDateTime(4);

        Patient newPatient = new Patient(name, userName, password, dob, id);
        patients.Add(newPatient);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return patients;
    }

    public static List<Doctor> SearchByName(string searchQuery)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM doctors WHERE name LIKE @SearchQuery", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@SearchQuery", "%" + searchQuery + "%"));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Doctor> matches = new List<Doctor>{};
      while (rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string username = rdr.GetString(2);
        string password = rdr.GetString(3);
        string specialty = rdr.GetString(4);
        Doctor newDoctor = new Doctor(name, username, password, specialty, id);
        matches.Add(newDoctor);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return matches;
    }

    public static List<Doctor> SearchBySpecialty(string searchQuery)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM doctors WHERE specialty LIKE @SearchQuery", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@SearchQuery", "%" + searchQuery + "%"));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Doctor> matches = new List<Doctor>{};
      while (rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string username = rdr.GetString(2);
        string password = rdr.GetString(3);
        string specialty = rdr.GetString(4);
        Doctor newDoctor = new Doctor(name, username, password, specialty, id);
        matches.Add(newDoctor);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return matches;
    }

    public List<Appointment> GetAppointments()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM appointments WHERE doctor_id = @DoctorId", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@DoctorId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Appointment> appointments = new List<Appointment>{};
      while (rdr.Read())
      {
        Appointment newAppointment = new Appointment(rdr.GetDateTime(1), rdr.GetInt32(3), rdr.GetInt32(2), rdr.GetString(4), rdr.GetInt32(0));
        appointments.Add(newAppointment);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return appointments;
    }

    public void DeleteSingleDoctor()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM doctors WHERE id = @DoctorId; DELETE FROM doctors_patients WHERE doctor_id = @DoctorId; DELETE FROM appointments WHERE doctor_id = @DoctorId;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@DoctorId", this.Id));
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }

    public void Update(string name, string username, string password, string specialty)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("UPDATE doctors SET name = @DoctorName, username = @DoctorUsername, password = @DoctorPassword, specialty = @DoctorSpecialty OUTPUT INSERTED.name, INSERTED.username, INSERTED.password, INSERTED.specialty WHERE id = @DoctorId;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@DoctorName", name));
      cmd.Parameters.Add(new SqlParameter("@DoctorUsername", username));
      cmd.Parameters.Add(new SqlParameter("@DoctorPassword", password));
      cmd.Parameters.Add(new SqlParameter("@DoctorSpecialty", specialty));
      cmd.Parameters.Add(new SqlParameter("@DoctorId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this.Name = rdr.GetString(0);
        this.UserName = rdr.GetString(1);
        this.Password = rdr.GetString(2);
        this.Specialty = rdr.GetString(3);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();
    }

    public static List<Doctor> Login(string username, string password)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM doctors WHERE username = @DoctorUsername AND password = @DoctorPassword", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@DoctorUsername", username));
      cmd.Parameters.Add(new SqlParameter("@DoctorPassword", password));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Doctor> loggedIn = new List<Doctor>{};
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string DoctorUsername = rdr.GetString(2);
        string DoctorPassword = rdr.GetString(3);
        string specialty = rdr.GetString(4);
        Doctor newDoctor = new Doctor(name, username, password, specialty, id);
        loggedIn.Add(newDoctor);
      }

      if (rdr != null)
      {
          rdr.Close();
      }
      DB.CloseConnection();

      return loggedIn;
    }

    public void DeletePatients()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM doctors_patients WHERE doctor_id = @DoctorId", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@DoctorId", this.Id));

      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }

    public void DeletePatientRelationship(Patient toDelete)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM doctors_patients WHERE patient_id = @PatientId;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@PatientId", toDelete.Id));
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }

    public static void DeleteAll()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM doctors; DELETE FROM doctors_patients; DELETE FROM appointments;", DB.GetConnection());
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }
  }
}
