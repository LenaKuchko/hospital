using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Hospital.Objects
{
  public class Appointment
  {
    public int Id {get; set;}
    public int PatientId {get; set;}
    public int DoctorId {get; set;}
    public DateTime Date {get; set;}
    public string Description {get; set;}


    public Appointment()
    {
      Id = 0;
      PatientId = 0;
      DoctorId = 0;
      Date = default(DateTime);
      Description = null;
     }

    public Appointment(DateTime date, int doctorId, int patientId, string description, int id = 0)
    {
      Id = id;
      DoctorId = doctorId;
      PatientId = patientId;
      Date = date;
      Description = description;
    }

    public override bool Equals(System.Object otherAppointment)
    {
      if (!(otherAppointment is Appointment))
      {
        return false;
      }
      else
      {
        Appointment newAppointment = (Appointment) otherAppointment;
        return (this.Id == newAppointment.Id &&
                this.PatientId == newAppointment.PatientId &&
                this.DoctorId == newAppointment.DoctorId &&
                this.Date == newAppointment.Date &&
                this.Description == newAppointment.Description);
      }
    }

    public static List<Appointment>GetAll()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM appointments;", DB.GetConnection());
      SqlDataReader rdr = cmd.ExecuteReader();

      List<Appointment> allAppointments = new List<Appointment>{};
      while (rdr.Read())
      {
        int id = rdr.GetInt32(0);
        DateTime date = rdr.GetDateTime(1);
        int patientId = rdr.GetInt32(2);
        int doctorId = rdr.GetInt32(3);
        string description = rdr.GetString(4);

        Appointment newAppointment = new Appointment(date, doctorId, patientId, description, id);
        allAppointments.Add(newAppointment);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();

      return allAppointments;
    }

    public void Save()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("INSERT INTO appointments (date, patient_id, doctor_id, description) OUTPUT INSERTED.id VALUES (@Date, @PatientId, @DoctorId, @Description);", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@Date", this.Date));
      cmd.Parameters.Add(new SqlParameter("@PatientId", this.PatientId));
      cmd.Parameters.Add(new SqlParameter("@DoctorId", this.DoctorId));
      cmd.Parameters.Add(new SqlParameter("@Description", this.Description));

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

    public static Appointment Find(int searchId)
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("SELECT * FROM appointments WHERE id = @AppointmentId;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@AppointmentId", searchId));

      SqlDataReader rdr = cmd.ExecuteReader();

      Appointment foundAppointment = new Appointment();
      while (rdr.Read())
      {
        foundAppointment.Id = rdr.GetInt32(0);
        foundAppointment.Date = rdr.GetDateTime(1);
        foundAppointment.PatientId = rdr.GetInt32(2);
        foundAppointment.DoctorId = rdr.GetInt32(3);
        foundAppointment.Description = rdr.GetString(4);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      DB.CloseConnection();
      return foundAppointment;
    }

    public void DeleteSingleAppointment()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM appointments WHERE id = @AppointmentId;", DB.GetConnection());

      cmd.Parameters.Add(new SqlParameter("@AppointmentId", this.Id));
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }

    public static void DeleteAll()
    {
      DB.CreateConnection();
      DB.OpenConnection();

      SqlCommand cmd = new SqlCommand("DELETE FROM appointments;", DB.GetConnection());
      cmd.ExecuteNonQuery();
      DB.CloseConnection();
    }
  }
}
