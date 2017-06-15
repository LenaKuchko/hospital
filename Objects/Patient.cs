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

    public Patient()
    {
      Id = 0;
      Name = null;
      UserName = null;
      Password = null;
      DOB = default(DateTime);
     }

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

    public static Patient Find(int searchId)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM patients WHERE id = @PatientId;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@PatientId", searchId));

      SqlDataReader rdr = cmd.ExecuteReader();

      Patient foundPatient = new Patient();
      while (rdr.Read())
      {
        foundPatient.Id = rdr.GetInt32(0);
        foundPatient.Name = rdr.GetString(1);
        foundPatient.UserName = rdr.GetString(2);
        foundPatient.Password = rdr.GetString(3);
        foundPatient.DOB = rdr.GetDateTime(4);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return foundPatient;
    }

    public void AddDoctor(Doctor doctorToAdd)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("INSERT INTO doctors_patients (doctor_id, patient_id) VALUES (@DoctorId, @PatientId);", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@DoctorId", doctorToAdd.Id));
      cmd.Parameters.Add(new SqlParameter("@PatientId", this.Id));

      cmd.ExecuteNonQuery();

      DB.CloseConnection();
    }

    public List<Doctor> GetDoctors()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT doctors.* FROM patients JOIN doctors_patients ON (patients.id = doctors_patients.patient_id) JOIN doctors ON (doctors.id = doctors_patients.doctor_id) WHERE patient_id = @PatientId;", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@PatientId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Doctor> doctors = new List<Doctor>{};
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string userName = rdr.GetString(2);
        string password = rdr.GetString(3);
        string speciality = rdr.GetString(4);

        Doctor newDoctor = new Doctor(name, userName, password, speciality, id);
        doctors.Add(newDoctor);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return doctors;
    }

    public void CreateAppointment(DateTime time, Doctor doctor, string description)
    {
      Appointment newAppointment = new Appointment(time, doctor.Id, this.Id, description);
      newAppointment.Save();
    }

    public List<Appointment> GetAppointments()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM appointments WHERE patient_id = @PatientId", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@PatientId", this.Id));

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

    public static List<Patient> SearchByName(string searchQuery)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM patients WHERE name LIKE @SearchQuery", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@SearchQuery", "%" + searchQuery + "%"));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Patient> matches = new List<Patient>{};
      while (rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string username = rdr.GetString(2);
        string password = rdr.GetString(3);
        DateTime dob = rdr.GetDateTime(4);
        Patient newPatient = new Patient(name, username, password, dob, id);
        matches.Add(newPatient);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return matches;
    }

    public void DeleteSinglePatient()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM patients WHERE id = @PatientId; DELETE FROM doctors_patients WHERE patient_id = @PatientId; DELETE FROM appointments WHERE patient_id = @PatientId;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@PatientId", this.Id));
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }

    public void Update(string name, string username, string password)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("UPDATE patients SET name = @PatientName, username = @PatientUsername, password = @PatientPassword OUTPUT INSERTED.name, INSERTED.username, INSERTED.password WHERE id = @PatientId;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@PatientName", name));
      cmd.Parameters.Add(new SqlParameter("@PatientUsername", username));
      cmd.Parameters.Add(new SqlParameter("@PatientPassword", password));
      cmd.Parameters.Add(new SqlParameter("@PatientId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this.Name = rdr.GetString(0);
        this.UserName = rdr.GetString(1);
        this.Password = rdr.GetString(2);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();
    }

    public bool Login(string username, string password)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM patients WHERE id = @PatientId", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@PatientId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      string patientUsername = null;
      string patientPassword = null;

      while(rdr.Read())
      {
        patientUsername = rdr.GetString(2);
        patientPassword = rdr.GetString(3);
      }

      if (rdr != null)
      {
          rdr.Close();
      }
      DB.CloseConnection();

      return (patientUsername == username && patientPassword == password);
    }

    public void DeleteDoctors()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM doctors_patients WHERE patient_id = @PatientId", DB.GetConnection());
      cmd.Parameters.Add(new SqlParameter("@PatientId", this.Id));

      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }

    public void DeleteAppointments()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM appointments WHERE patient_id = @PatientId", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@PatientId", this.Id));
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }

    public void DeleteSingleAppointment(Appointment toDelete)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM appointments WHERE id = @AppointmentId;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@AppointmentId", toDelete.Id));
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }

    public void DeleteDoctorRelationship(Doctor toDelete)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM doctors_patients WHERE doctor_id = @DoctorId;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@DoctorId", toDelete.Id));
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }

    public List<Appointment> GetMissedAppointments(DateTime now)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM appointments WHERE patient_id = @PatientId AND date < @CurrentDate;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@PatientId", this.Id));
      cmd.Parameters.Add(new SqlParameter("@CurrentDate", now));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Appointment> missedAppointments = new List<Appointment>{};
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        DateTime date = rdr.GetDateTime(1);
        int patientId = rdr.GetInt32(2);
        int doctorId = rdr.GetInt32(3);
        string description = rdr.GetString(4);

        Appointment newAppointment = new Appointment(date, doctorId, patientId, description, id);
        missedAppointments.Add(newAppointment);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return missedAppointments;
    }

    public static void DeleteAll()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM patients; DELETE FROM doctors_patients; DELETE FROM appointments;", DB.GetConnection());
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }
  }
}
