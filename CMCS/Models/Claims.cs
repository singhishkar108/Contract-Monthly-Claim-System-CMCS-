using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace CMCS.Models
{
    public class Claims
    {
        //unique identifier for the claim
        public int Id { get; set; }

        //Id of the lecturer submitting the claim
        [Required]
        public string LecturerID { get; set; }

        //first name of the lecturer
        [Required]
        public string FirstName { get; set; }

        //last name of the lecturer
        [Required]
        public string LastName { get; set; }

        //start date of the claims period
        [Required]
        public DateTime ClaimsPeriodStart { get; set; }

        //end date of the claims period
        [Required]
        public DateTime ClaimsPeriodEnd { get; set; }

        //total hours worked during the claims period
        [Required]
        public double HoursWorked { get; set; }

        //rate per hour for the work done
        [Required]
        public double RatePerHour { get; set; }

        //total amount claimed, calculated as HoursWorked * RatePerHour
        [Required]
        public double TotalAmount { get; set; }

        //description of the work done
        public string DescriptionOfWork { get; set; }

        //list of supporting documents for the claim
        //not mapped to the database, used for handling file uploads
        [Required]
        [DataType(DataType.MultilineText)]
        public string SupportingDocuments { get; set; }

        //UserID of the person submitting or associated with the claim.
        [Required]
        public string UserID { get; set; }

        //status of the claim, such as "Pending", "Approved", or "Rejected".
        [Required]
        [StringLength(20)]
        public string ClaimStatus { get; set; } = "Pending"; //set default status to "Pending"
        
        //property to store the date and time the claim was submitted
        public DateTime DateSubmitted { get; set; } = DateTime.Now;  //automatically set to the current date and time

        //collection to store the files associated with this claim.
        //a claim can have multiple related files.
        public virtual ICollection<Files> File { get; set; }

        //constructor to initialize the 'File' collection to prevent null reference exceptions
        public Claims()
        {
            File = new HashSet<Files>();
        }

        //property to store the number of overtime hours worked
        public double? OvertimeHours { get; set; }

        //property to store the rate of pay for overtime hours
        public double? OvertimeRate { get; set; }

        //calculation total overtime pay
        //?? handles null values in OvertimeHours and OvertimeRate
        //if OvertimeHours || OvertimeRate = null, it defaults to 0
        public double OvertimePay => (OvertimeHours ?? 0) * (OvertimeRate ?? 0);

    }
}
